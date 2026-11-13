# Lunex Common SQL Queries Reference

## Overview

This document contains commonly used SQL queries for the Lunex application, organized by functional area. All queries use the `lunex` schema and snake_case naming convention.

## Authentication Queries

### User Registration (Transactional)
```sql
-- Execute as single transaction
BEGIN;

-- 1. Insert user
INSERT INTO lunex.users (
    id, name, email, image_url, password_hash, password_salt
) VALUES (
    'user-guid-here',
    'John Doe',
    'john.doe@example.com',
    'https://api.dicebear.com/7.x/avataaars/svg?seed=john.doe',
    '\xhashbytes...',  -- HMAC-SHA512 hash
    '\xsaltbytes...'   -- Random salt
);

-- 2. Insert member profile
INSERT INTO lunex.members (
    id, date_of_birth, image_url, name, gender, description, city, country, created_at, last_active_at
) VALUES (
    'user-guid-here',  -- Same ID as user (1:1 relationship)
    '1990-01-15',
    'https://api.dicebear.com/7.x/avataaars/svg?seed=johndoe',
    'John Doe',
    'Male',
    'Software developer',
    'San Francisco',
    'USA',
    NOW(),
    NOW()
);

COMMIT;
```

### User Login Verification
```sql
-- Get user credentials for password verification
SELECT 
    id, 
    name, 
    email, 
    image_url, 
    password_hash, 
    password_salt
FROM lunex.users
WHERE email = $1;  -- Parameterized query
```

### Check Email Availability
```sql
-- Returns true if email exists (case-insensitive)
SELECT EXISTS (
    SELECT 1 FROM lunex.users 
    WHERE LOWER(email) = LOWER($1)
);
```

### Get User by ID
```sql
SELECT id, name, email, image_url
FROM lunex.users
WHERE id = $1;
```

## Member Profile Queries

### Get All Members (Paginated)
```sql
SELECT 
    m.id, m.date_of_birth, m.image_url, m.name, m.gender,
    m.description, m.city, m.country, m.created_at, m.last_active_at,
    COALESCE(
        json_agg(
            json_build_object(
                'id', p.id,
                'url', p.url,
                'publicId', p.public_id
            )
        ) FILTER (WHERE p.id IS NOT NULL),
        '[]'::json
    ) AS photos
FROM lunex.members m
LEFT JOIN lunex.photos p ON p.member_id = m.id
GROUP BY m.id
ORDER BY m.created_at DESC
LIMIT $1 OFFSET $2;  -- $1 = pageSize, $2 = (page-1)*pageSize
```

### Get Member by ID (with Photos)
```sql
SELECT 
    m.id, m.date_of_birth, m.image_url, m.name, m.gender,
    m.description, m.city, m.country, m.created_at, m.last_active_at,
    COALESCE(
        json_agg(
            json_build_object(
                'id', p.id,
                'url', p.url,
                'publicId', p.public_id
            )
        ) FILTER (WHERE p.id IS NOT NULL),
        '[]'::json
    ) AS photos
FROM lunex.members m
LEFT JOIN lunex.photos p ON p.member_id = m.id
WHERE m.id = $1
GROUP BY m.id;
```

### Get Member Photos Only
```sql
SELECT id, url, public_id
FROM lunex.photos
WHERE member_id = $1
ORDER BY id;
```

### Update Member Profile
```sql
UPDATE lunex.members
SET 
    name = $2,
    description = $3,
    city = $4,
    country = $5,
    last_active_at = NOW()
WHERE id = $1
RETURNING 
    id, date_of_birth, image_url, name, gender,
    description, city, country, created_at, last_active_at;
```

### Update Member Last Active
```sql
-- Call on each authenticated request
UPDATE lunex.members
SET last_active_at = NOW()
WHERE id = $1;
```

### Search Members
```sql
-- Search by name (case-insensitive partial match)
SELECT 
    id, date_of_birth, image_url, name, gender,
    description, city, country, created_at, last_active_at
FROM lunex.members
WHERE name ILIKE '%' || $1 || '%'
ORDER BY 
    CASE WHEN name ILIKE $1 || '%' THEN 0 ELSE 1 END,  -- Exact prefix first
    name
LIMIT $2 OFFSET $3;
```

### Advanced Member Search
```sql
-- Multi-field search with filters
SELECT 
    m.id, m.date_of_birth, m.image_url, m.name, m.gender,
    m.description, m.city, m.country, m.created_at, m.last_active_at
FROM lunex.members m
WHERE 
    ($1::text IS NULL OR m.name ILIKE '%' || $1 || '%')  -- name filter
    AND ($2::text IS NULL OR m.gender = $2)              -- gender filter
    AND ($3::text IS NULL OR m.city ILIKE '%' || $3 || '%')  -- city filter
    AND ($4::text IS NULL OR m.country ILIKE '%' || $4 || '%')  -- country filter
    AND ($5::date IS NULL OR m.date_of_birth >= $5)      -- min age (birth date)
    AND ($6::date IS NULL OR m.date_of_birth <= $6)      -- max age (birth date)
ORDER BY m.last_active_at DESC
LIMIT $7 OFFSET $8;
```

## Photo Management Queries

### Upload Photo (Add to Member)
```sql
INSERT INTO lunex.photos (id, url, public_id, member_id, user_id)
VALUES (
    'photo-guid-here',
    'https://res.cloudinary.com/cloud/image/upload/v123/members/user-id/photo-guid.jpg',
    'members/user-id/photo-guid',
    'member-guid-here',
    NULL  -- user_id only for user-only photos
)
RETURNING id, url, public_id;
```

### Delete Photo (with Ownership Check)
```sql
-- Returns public_id for Cloudinary deletion, then deletes record
WITH deleted AS (
    DELETE FROM lunex.photos
    WHERE id = $1 AND member_id = $2
    RETURNING public_id
)
SELECT public_id FROM deleted;
```

### Get Photo by ID (with Ownership)
```sql
SELECT p.id, p.url, p.public_id, p.member_id
FROM lunex.photos p
WHERE p.id = $1 AND p.member_id = $2;
```

### Get All Photos for Member (with Member Info)
```sql
SELECT 
    p.id, p.url, p.public_id,
    m.name AS member_name, m.id AS member_id
FROM lunex.photos p
JOIN lunex.members m ON m.id = p.member_id
WHERE p.member_id = $1
ORDER BY p.id;
```

## Administrative Queries

### User Management

#### List All Users (Admin)
```sql
SELECT 
    u.id, u.name, u.email, u.image_url,
    m.date_of_birth, m.gender, m.city, m.country,
    m.created_at, m.last_active_at,
    COUNT(p.id) AS photo_count
FROM lunex.users u
LEFT JOIN lunex.members m ON m.id = u.id
LEFT JOIN lunex.photos p ON p.member_id = m.id
GROUP BY u.id, m.id
ORDER BY u.email;
```

#### Deactivate User (Soft Delete)
```sql
-- Option 1: Mark as inactive (add is_active column if needed)
-- Option 2: Hard delete (cascades to member and photos)
DELETE FROM lunex.users WHERE id = $1;
-- CASCADE DELETE will remove member and photos automatically
```

#### Reset User Password
```sql
UPDATE lunex.users
SET 
    password_hash = $2,  -- New HMAC-SHA512 hash
    password_salt = $3   -- New salt
WHERE id = $1;
```

### Statistics & Analytics

#### Member Count by Country
```sql
SELECT 
    country,
    COUNT(*) AS member_count
FROM lunex.members
GROUP BY country
ORDER BY member_count DESC;
```

#### Member Count by Gender
```sql
SELECT 
    gender,
    COUNT(*) AS member_count
FROM lunex.members
GROUP BY gender;
```

#### Age Distribution
```sql
SELECT 
    CASE 
        WHEN age < 18 THEN 'Under 18'
        WHEN age BETWEEN 18 AND 24 THEN '18-24'
        WHEN age BETWEEN 25 AND 34 THEN '25-34'
        WHEN age BETWEEN 35 AND 44 THEN '35-44'
        WHEN age BETWEEN 45 AND 54 THEN '45-54'
        WHEN age BETWEEN 55 AND 64 THEN '55-64'
        ELSE '65+'
    END AS age_group,
    COUNT(*) AS member_count
FROM (
    SELECT 
        DATE_PART('year', AGE(date_of_birth))::int AS age
    FROM lunex.members
) ages
GROUP BY age_group
ORDER BY MIN(age);
```

#### Activity Statistics
```sql
-- Members active in last 7 days
SELECT COUNT(*) FROM lunex.members
WHERE last_active_at > NOW() - INTERVAL '7 days';

-- Members active in last 30 days
SELECT COUNT(*) FROM lunex.members
WHERE last_active_at > NOW() - INTERVAL '30 days';

-- New members this month
SELECT COUNT(*) FROM lunex.members
WHERE created_at > DATE_TRUNC('month', NOW());

-- Average photos per member
SELECT AVG(photo_count) FROM (
    SELECT COUNT(p.id) AS photo_count
    FROM lunex.members m
    LEFT JOIN lunex.photos p ON p.member_id = m.id
    GROUP BY m.id
) sub;
```

#### Photo Statistics
```sql
-- Total photos
SELECT COUNT(*) FROM lunex.photos;

-- Photos by member (top 10)
SELECT 
    m.name,
    m.id,
    COUNT(p.id) AS photo_count
FROM lunex.members m
LEFT JOIN lunex.photos p ON p.member_id = m.id
GROUP BY m.id, m.name
ORDER BY photo_count DESC
LIMIT 10;

-- Photos with Cloudinary public_id (for cleanup)
SELECT id, public_id, member_id
FROM lunex.photos
WHERE public_id IS NOT NULL;
```

### Data Quality & Maintenance

#### Find Orphaned Records
```sql
-- Photos without valid member
SELECT p.* FROM lunex.photos p
LEFT JOIN lunex.members m ON m.id = p.member_id
WHERE m.id IS NULL;

-- Users without member profile
SELECT u.* FROM lunex.users u
LEFT JOIN lunex.members m ON m.id = u.id
WHERE m.id IS NULL;

-- Members without user (should not happen with FK)
SELECT m.* FROM lunex.members m
LEFT JOIN lunex.users u ON u.id = m.id
WHERE u.id IS NULL;
```

#### Cleanup Orphaned Photos
```sql
-- Delete photos with no valid member
DELETE FROM lunex.photos
WHERE member_id NOT IN (SELECT id FROM lunex.members);
```

#### Find Duplicate Emails
```sql
SELECT email, COUNT(*) 
FROM lunex.users 
GROUP BY email 
HAVING COUNT(*) > 1;
```

#### Members with Missing Data
```sql
-- Members with empty description
SELECT id, name FROM lunex.members WHERE description = '' OR description IS NULL;

-- Members with default image
SELECT id, name FROM lunex.members WHERE image_url IS NULL OR image_url = '';

-- Members never active (last_active_at = created_at)
SELECT id, name, created_at, last_active_at 
FROM lunex.members 
WHERE last_active_at = created_at;
```

## Migration & Deployment Queries

### Pre-Migration Checks
```sql
-- Check current schema version
SELECT * FROM lunex."__EFMigrationsHistory" ORDER BY "MigrationId";

-- Check table sizes before migration
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'lunex';
```

### Post-Migration Verification
```sql
-- Verify new columns exist
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_schema = 'lunex' AND table_name = 'members'
ORDER BY ordinal_position;

-- Verify indexes
SELECT indexname, indexdef
FROM pg_indexes
WHERE schemaname = 'lunex';

-- Verify foreign keys
SELECT
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY' AND tc.table_schema = 'lunex';
```

### Rollback Queries (Emergency)
```sql
-- Restore from backup point (requires backup)
-- See schema.md for backup/restore commands

-- Manual rollback example (if migration added column)
ALTER TABLE lunex.members DROP COLUMN IF EXISTS new_column;

-- Recreate dropped index
CREATE INDEX IF NOT EXISTS ix_members_name ON lunex.members (name);
```

## Performance Monitoring Queries

### Slow Query Analysis
```sql
-- Requires pg_stat_statements extension
SELECT 
    query,
    calls,
    total_exec_time,
    mean_exec_time,
    rows
FROM pg_stat_statements
WHERE query LIKE '%lunex%'
    AND mean_exec_time > 100  -- > 100ms average
ORDER BY mean_exec_time DESC
LIMIT 20;
```

### Index Usage
```sql
SELECT 
    schemaname,
    tablename,
    indexname,
    idx_tup_read,
    idx_tup_fetch,
    idx_scan
FROM pg_stat_user_indexes
WHERE schemaname = 'lunex'
ORDER BY idx_scan DESC;
```

### Table Bloat Check
```sql
-- Approximate table bloat
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS total_size,
    pg_size_pretty(pg_relation_size(schemaname||'.'||tablename)) AS table_size,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename) - pg_relation_size(schemaname||'.'||tablename)) AS index_size
FROM pg_tables
WHERE schemaname = 'lunex';
```

### Connection Pool Status
```sql
SELECT 
    count(*) AS total_connections,
    count(*) FILTER (WHERE state = 'active') AS active,
    count(*) FILTER (WHERE state = 'idle') AS idle,
    count(*) FILTER (WHERE state = 'idle in transaction') AS idle_in_tx
FROM pg_stat_activity
WHERE datname = 'lunex';
```

## Testing Queries

### Test Data Setup
```sql
-- Create test user
INSERT INTO lunex.users (id, name, email, image_url, password_hash, password_salt)
VALUES ('test-user-1', 'Test User', 'test@example.com', '', '\xhash', '\xsalt');

-- Create test member
INSERT INTO lunex.members (id, date_of_birth, image_url, name, gender, description, city, country, created_at, last_active_at)
VALUES ('test-user-1', '1990-01-01', '', 'Test User', 'Other', 'Test description', 'Test City', 'Test Country', NOW(), NOW());

-- Create test photos
INSERT INTO lunex.photos (id, url, public_id, member_id)
VALUES 
    ('test-photo-1', 'https://example.com/photo1.jpg', 'test/photo1', 'test-user-1'),
    ('test-photo-2', 'https://example.com/photo2.jpg', 'test/photo2', 'test-user-1');
```

### Test Data Cleanup
```sql
-- Remove test data
DELETE FROM lunex.photos WHERE member_id = 'test-user-1';
DELETE FROM lunex.members WHERE id = 'test-user-1';
DELETE FROM lunex.users WHERE id = 'test-user-1';
```

### Verify Test State
```sql
-- Check test data exists
SELECT 'users' AS table, COUNT(*) FROM lunex.users WHERE id = 'test-user-1'
UNION ALL SELECT 'members', COUNT(*) FROM lunex.members WHERE id = 'test-user-1'
UNION ALL SELECT 'photos', COUNT(*) FROM lunex.photos WHERE member_id = 'test-user-1';
```

## Query Patterns for Application Code

### Parameterized Query Templates (for EF Core / Dapper)

```csharp
// GetMemberByIdAsync
const string GetMemberByIdSql = @"
    SELECT m.id, m.date_of_birth, m.image_url, m.name, m.gender,
           m.description, m.city, m.country, m.created_at, m.last_active_at,
           COALESCE(json_agg(json_build_object('id', p.id, 'url', p.url, 'publicId', p.public_id)) 
               FILTER (WHERE p.id IS NOT NULL), '[]'::json) AS photos
    FROM lunex.members m
    LEFT JOIN lunex.photos p ON p.member_id = m.id
    WHERE m.id = @Id
    GROUP BY m.id";

// GetMembersAsync (paginated)
const string GetMembersSql = @"
    SELECT m.id, m.date_of_birth, m.image_url, m.name, m.gender,
           m.description, m.city, m.country, m.created_at, m.last_active_at
    FROM lunex.members m
    ORDER BY m.created_at DESC
    LIMIT @PageSize OFFSET @Offset";

// SearchMembersAsync
const string SearchMembersSql = @"
    SELECT m.id, m.date_of_birth, m.image_url, m.name, m.gender,
           m.description, m.city, m.country, m.created_at, m.last_active_at
    FROM lunex.members m
    WHERE m.name ILIKE '%' || @SearchTerm || '%'
    ORDER BY m.name
    LIMIT @PageSize OFFSET @Offset";
```

## Security Notes

1. **Always use parameterized queries** - Never concatenate user input
2. **Validate ownership** - Check `member_id` matches authenticated user for write operations
3. **Limit result sets** - Always use `LIMIT` for queries that could return many rows
4. **Use transactions** - For multi-table operations (registration, photo upload)
5. **Hash passwords** - Never store plain text; use HMAC-SHA512 with unique salt per user
6. **Sanitize search input** - Escape wildcards in ILIKE patterns if needed

---

*Last Updated: 2026-02-09*
*Schema Version: 1.0.0 (InitialCommit migration)*