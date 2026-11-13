# Lunex Database Schema Documentation

## Overview

**Database**: PostgreSQL 15+
**Schema**: `lunex`
**Naming Convention**: snake_case (via EF Core `UseSnakeCaseNamingConvention()`)
**Migration History**: Managed by EF Core in `Lunex.Infrastructure`

## Entity Relationship Diagram

```
┌─────────────────────┐       ┌─────────────────────┐       ┌─────────────────────┐
│      users          │       │      members        │       │      photos         │
├─────────────────────┤       ├─────────────────────┤       ├─────────────────────┤
│ id (PK)             │◄──────│ id (PK, FK)         │◄──────│ id (PK)             │
│ name                │  1:1  │ date_of_birth       │  1:N  │ url                 │
│ email (UNIQUE)      │       │ image_url           │       │ public_id           │
│ image_url           │       │ name                │       │ member_id (FK)      │
│ password_hash       │       │ gender              │       │ user_id (FK, NULL)  │
│ password_salt       │       │ description         │       └─────────────────────┘
└─────────────────────┘       │ city                │
                              │ country             │
                              │ created_at          │
                              │ last_active_at      │
                              └─────────────────────┘
```

## Table Definitions

### 1. users Table

```sql
CREATE TABLE lunex.users (
    id           TEXT PRIMARY KEY,
    name         TEXT NOT NULL,
    email        TEXT NOT NULL UNIQUE,
    image_url    TEXT,
    password_hash BYTEA NOT NULL,
    password_salt BYTEA NOT NULL
);

-- Indexes
CREATE UNIQUE INDEX ix_users_email ON lunex.users (email);
```

**Column Details:**

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| id | TEXT | NO | gen_random_uuid()::text | Primary key (GUID as string) |
| name | TEXT | NO | - | User's full name |
| email | TEXT | NO | - | Unique email address |
| image_url | TEXT | YES | NULL | Profile image URL |
| password_hash | BYTEA | NO | - | HMAC-SHA512 hash of password |
| password_salt | BYTEA | NO | - | Random salt for password hashing |

**Constraints:**
- Primary Key: `pk_users` on `id`
- Unique: `email` (enforced by unique index)

### 2. members Table

```sql
CREATE TABLE lunex.members (
    id             TEXT PRIMARY KEY,
    date_of_birth  DATE NOT NULL,
    image_url      TEXT,
    name           TEXT NOT NULL,
    gender         TEXT NOT NULL,
    description    TEXT NOT NULL DEFAULT '',
    city           TEXT NOT NULL,
    country        TEXT NOT NULL,
    created_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_active_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_members_users_id 
        FOREIGN KEY (id) REFERENCES lunex.users (id) 
        ON DELETE CASCADE
);

-- Indexes
CREATE INDEX ix_members_name ON lunex.members (name);
CREATE INDEX ix_members_city_country ON lunex.members (city, country);
```

**Column Details:**

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| id | TEXT | NO | - | Primary key, FK to users.id (1:1) |
| date_of_birth | DATE | NO | - | Member's birth date |
| image_url | TEXT | YES | NULL | Profile image URL |
| name | TEXT | NO | - | Member's display name |
| gender | TEXT | NO | - | Member's gender |
| description | TEXT | NO | '' | Member's bio/description |
| city | TEXT | NO | - | Member's city |
| country | TEXT | NO | - | Member's country |
| created_at | TIMESTAMPTZ | NO | NOW() | Record creation timestamp |
| last_active_at | TIMESTAMPTZ | NO | NOW() | Last activity timestamp |

**Constraints:**
- Primary Key: `pk_members` on `id`
- Foreign Key: `fk_members_users_id` references `users(id)` with CASCADE DELETE

### 3. photos Table

```sql
CREATE TABLE lunex.photos (
    id        TEXT PRIMARY KEY,
    url       TEXT NOT NULL,
    public_id TEXT,
    member_id TEXT NOT NULL,
    user_id   TEXT,

    CONSTRAINT fk_photos_members_member_id 
        FOREIGN KEY (member_id) REFERENCES lunex.members (id) 
        ON DELETE CASCADE,
    CONSTRAINT fk_photos_users_user_id 
        FOREIGN KEY (user_id) REFERENCES lunex.users (id) 
        ON DELETE SET NULL
);

-- Indexes
CREATE INDEX ix_photos_member_id ON lunex.photos (member_id);
CREATE INDEX ix_photos_user_id ON lunex.photos (user_id);
```

**Column Details:**

| Column | Type | Nullable | Default | Description |
|--------|------|----------|---------|-------------|
| id | TEXT | NO | gen_random_uuid()::text | Primary key (GUID as string) |
| url | TEXT | NO | - | Cloudinary secure URL |
| public_id | TEXT | YES | NULL | Cloudinary public ID for deletion |
| member_id | TEXT | NO | - | FK to members.id (required) |
| user_id | TEXT | YES | NULL | FK to users.id (optional, for user-only photos) |

**Constraints:**
- Primary Key: `pk_photos` on `id`
- Foreign Key: `fk_photos_members_member_id` references `members(id)` with CASCADE DELETE
- Foreign Key: `fk_photos_users_user_id` references `users(id)` with SET NULL

## Migration History

### Initial Migration (20261112210754_InitialCommit)

```csharp
// Creates all three tables with:
// - lunex schema
// - Primary keys
// - Foreign keys with cascade rules
// - Indexes on foreign keys
// - Snake_case naming
```

**Up Migration:**
```sql
CREATE SCHEMA lunex;

CREATE TABLE lunex.users (...);
CREATE TABLE lunex.members (...);
CREATE TABLE lunex.photos (...);

CREATE INDEX ix_photos_member_id ON lunex.photos (member_id);
CREATE INDEX ix_photos_user_id ON lunex.photos (user_id);
```

**Down Migration:**
```sql
DROP TABLE lunex.photos;
DROP TABLE lunex.members;
DROP TABLE lunex.users;
DROP SCHEMA lunex;
```

## Common Queries

### User Management

#### Get User by Email (Login)
```sql
SELECT id, name, email, image_url, password_hash, password_salt
FROM lunex.users
WHERE email = $1;
```

#### Check Email Exists (Registration)
```sql
SELECT EXISTS(
    SELECT 1 FROM lunex.users 
    WHERE LOWER(email) = LOWER($1)
);
```

#### Create User with Member (Registration)
```sql
-- Transaction required
BEGIN;

INSERT INTO lunex.users (id, name, email, image_url, password_hash, password_salt)
VALUES ($1, $2, $3, $4, $5, $6);

INSERT INTO lunex.members (id, date_of_birth, image_url, name, gender, description, city, country, created_at, last_active_at)
VALUES ($1, $7, $8, $2, $9, $10, $11, $12, NOW(), NOW());

COMMIT;
```

### Member Queries

#### Get All Members (with Photos)
```sql
SELECT 
    m.id, m.date_of_birth, m.image_url, m.name, m.gender, 
    m.description, m.city, m.country, m.created_at, m.last_active_at,
    json_agg(
        json_build_object(
            'id', p.id,
            'url', p.url,
            'publicId', p.public_id
        )
    ) FILTER (WHERE p.id IS NOT NULL) AS photos
FROM lunex.members m
LEFT JOIN lunex.photos p ON p.member_id = m.id
GROUP BY m.id
ORDER BY m.created_at DESC;
```

#### Get Member by ID
```sql
SELECT 
    m.id, m.date_of_birth, m.image_url, m.name, m.gender, 
    m.description, m.city, m.country, m.created_at, m.last_active_at
FROM lunex.members m
WHERE m.id = $1;
```

#### Get Member with Photos
```sql
SELECT 
    m.*,
    p.id AS photo_id, p.url AS photo_url, p.public_id AS photo_public_id
FROM lunex.members m
LEFT JOIN lunex.photos p ON p.member_id = m.id
WHERE m.id = $1;
```

#### Update Member Profile
```sql
UPDATE lunex.members
SET name = $2,
    description = $3,
    city = $4,
    country = $5,
    last_active_at = NOW()
WHERE id = $1
RETURNING *;
```

#### Update Last Active Timestamp
```sql
UPDATE lunex.members
SET last_active_at = NOW()
WHERE id = $1;
```

### Photo Queries

#### Get Member Photos
```sql
SELECT id, url, public_id
FROM lunex.photos
WHERE member_id = $1
ORDER BY id;
```

#### Add Photo to Member
```sql
INSERT INTO lunex.photos (id, url, public_id, member_id, user_id)
VALUES ($1, $2, $3, $4, NULL)
RETURNING *;
```

#### Delete Photo
```sql
-- First get public_id for Cloudinary deletion
SELECT public_id FROM lunex.photos WHERE id = $1 AND member_id = $2;

-- Then delete from database
DELETE FROM lunex.photos 
WHERE id = $1 AND member_id = $2
RETURNING *;
```

#### Get Photo by ID (with ownership check)
```sql
SELECT p.*, m.id AS member_id
FROM lunex.photos p
JOIN lunex.members m ON m.id = p.member_id
WHERE p.id = $1 AND m.id = $2;
```

### Search & Discovery Queries

#### Search Members by Name
```sql
SELECT * FROM lunex.members
WHERE name ILIKE '%' || $1 || '%'
ORDER BY name
LIMIT $2 OFFSET $3;
```

#### Get Members by City/Country
```sql
SELECT * FROM lunex.members
WHERE city = $1 AND country = $2
ORDER BY created_at DESC;
```

#### Get Recently Active Members
```sql
SELECT * FROM lunex.members
WHERE last_active_at > NOW() - INTERVAL '7 days'
ORDER BY last_active_at DESC
LIMIT 20;
```

## Seed Data Queries

### Development Seed Data (from DatabaseSeeder)

```sql
-- Users (passwords: Admin@123, Password@123)
INSERT INTO lunex.users (id, name, email, image_url, password_hash, password_salt) VALUES
('uuid-1', 'Admin User', 'admin@lunex.com', 'https://api.dicebear.com/7.x/avataaars/svg?seed=admin', '\x...', '\x...'),
('uuid-2', 'John Doe', 'john.doe@lunex.com', 'https://api.dicebear.com/7.x/avataaars/svg?seed=john.doe', '\x...', '\x...'),
('uuid-3', 'Jane Smith', 'jane.smith@lunex.com', 'https://api.dicebear.com/7.x/avataaars/svg?seed=jane.smith', '\x...', '\x...'),
('uuid-4', 'Bob Wilson', 'bob.wilson@lunex.com', 'https://api.dicebear.com/7.x/avataaars/svg?seed=bob.wilson', '\x...', '\x...'),
('uuid-5', 'Alice Brown', 'alice.brown@lunex.com', 'https://api.dicebear.com/7.x/avataaars/svg?seed=alice.brown', '\x...', '\x...');

-- Members
INSERT INTO lunex.members (id, date_of_birth, image_url, name, gender, description, city, country, created_at, last_active_at) VALUES
('uuid-1', '1990-01-15', 'https://api.dicebear.com/7.x/avataaars/svg?seed=adminuser', 'Admin User', 'Male', 'System administrator', 'New York', 'USA', NOW(), NOW()),
('uuid-2', '1992-05-20', 'https://api.dicebear.com/7.x/avataaars/svg?seed=johndoe', 'John Doe', 'Male', 'Software developer passionate about clean code', 'San Francisco', 'USA', NOW(), NOW()),
('uuid-3', '1993-08-10', 'https://api.dicebear.com/7.x/avataaars/svg?seed=janesmith', 'Jane Smith', 'Female', 'UX designer who loves creating beautiful interfaces', 'London', 'UK', NOW(), NOW()),
('uuid-4', '1991-11-03', 'https://api.dicebear.com/7.x/avataaars/svg?seed=bobwilson', 'Bob Wilson', 'Male', 'Full-stack developer with 5+ years experience', 'Toronto', 'Canada', NOW(), NOW()),
('uuid-5', '1994-03-25', 'https://api.dicebear.com/7.x/avataaars/svg?seed=alicebrown', 'Alice Brown', 'Female', 'Data scientist and ML enthusiast', 'Sydney', 'Australia', NOW(), NOW());

-- Photos
INSERT INTO lunex.photos (id, url, public_id, member_id, user_id) VALUES
('photo-uuid-1', 'https://api.dicebear.com/7.x/avataaars/svg?seed=admin', 'admin_avatar', 'uuid-1', NULL),
('photo-uuid-2', 'https://api.dicebear.com/7.x/avataaars/svg?seed=john', 'john_avatar', 'uuid-2', NULL),
('photo-uuid-3', 'https://api.dicebear.com/7.x/avataaars/svg?seed=jane', 'jane_avatar', 'uuid-3', NULL),
('photo-uuid-4', 'https://api.dicebear.com/7.x/avataaars/svg?seed=bob', 'bob_avatar', 'uuid-4', NULL),
('photo-uuid-5', 'https://api.dicebear.com/7.x/avataaars/svg?seed=alice', 'alice_avatar', 'uuid-5', NULL);
```

## Maintenance Queries

### Database Statistics
```sql
-- Table sizes
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS total_size,
    pg_size_pretty(pg_relation_size(schemaname||'.'||tablename)) AS table_size
FROM pg_tables
WHERE schemaname = 'lunex';

-- Row counts
SELECT 'users' AS table, COUNT(*) FROM lunex.users
UNION ALL SELECT 'members', COUNT(*) FROM lunex.members
UNION ALL SELECT 'photos', COUNT(*) FROM lunex.photos;

-- Index usage
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_tup_read,
    idx_tup_fetch
FROM pg_stat_user_indexes
WHERE schemaname = 'lunex';
```

### Cleanup Queries

#### Remove Orphaned Photos (no member)
```sql
DELETE FROM lunex.photos
WHERE member_id NOT IN (SELECT id FROM lunex.members);
```

#### Update Member Last Active (batch)
```sql
UPDATE lunex.members
SET last_active_at = NOW()
WHERE last_active_at < NOW() - INTERVAL '1 year';
```

#### Find Users Without Members
```sql
SELECT u.* FROM lunex.users u
LEFT JOIN lunex.members m ON m.id = u.id
WHERE m.id IS NULL;
```

## Performance Considerations

### Recommended Indexes (Already Created)
- `ix_photos_member_id` - For member photo lookups
- `ix_photos_user_id` - For user photo lookups
- `ix_members_name` - For name searches
- `ix_members_city_country` - For location-based queries

### Query Optimization Tips
1. **Use LIMIT** for pagination
2. **Avoid SELECT *** - specify columns
3. **Use EXPLAIN ANALYZE** for slow queries
4. **Consider materialized views** for complex aggregations

### Connection Pooling
```csharp
// In Program.cs / DbContext configuration
options.UseNpgsql(connectionString, npgsqlOptions => 
    npgsqlOptions.EnableRetryOnFailure())
// Npgsql default pool: 1024 max connections
```

## Backup & Restore

### Full Backup
```bash
pg_dump -h localhost -U lunex_user -d lunex -n lunex > lunex_backup_$(date +%Y%m%d).sql
```

### Schema Only
```bash
pg_dump -h localhost -U lunex_user -d lunex -n lunex --schema-only > lunex_schema.sql
```

### Data Only
```bash
pg_dump -h localhost -U lunex_user -d lunex -n lunex --data-only > lunex_data.sql
```

### Restore
```bash
psql -h localhost -U lunex_user -d lunex < lunex_backup_20260115.sql
```

## Security

### Row Level Security (Future)
```sql
-- Enable RLS on members table
ALTER TABLE lunex.members ENABLE ROW LEVEL SECURITY;

-- Policy: users can only see their own member data
CREATE POLICY member_isolation ON lunex.members
    USING (id = current_setting('app.current_user_id')::text);
```

### Column Encryption (Sensitive Data)
```sql
-- For password_hash/password_salt, consider pgcrypto
-- Currently stored as BYTEA (binary) - not encrypted at rest
-- Consider: pgp_sym_encrypt for additional protection
```

## Monitoring Queries

### Slow Queries
```sql
-- Enable pg_stat_statements extension
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- View slow queries
SELECT query, calls, total_exec_time, mean_exec_time
FROM pg_stat_statements
WHERE query LIKE '%lunex%'
ORDER BY mean_exec_time DESC
LIMIT 20;
```

### Active Connections
```sql
SELECT pid, usename, application_name, client_addr, state, query_start
FROM pg_stat_activity
WHERE datname = 'lunex';
```

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 1.0.0 | 2026-11-12 | Initial schema: users, members, photos tables |
| 1.1.0 | 2026-02-09 | Added indexes for photo lookups and member search |

---

*Generated from EF Core migration: `20261112210754_InitialCommit`*