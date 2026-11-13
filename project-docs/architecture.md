# Lunex Project Architecture

## Overview

Lunex follows **Clean Architecture** principles with a clear separation of concerns across five .NET projects and an Angular 20 frontend.

## Solution Structure

```
Lunex.sln
├── src/
│   ├── Lunex.Api/              # Web API layer (Presentation)
│   ├── Lunex.Application/      # Application layer (Use cases, Services)
│   ├── Lunex.Contracts/        # Shared DTOs/Contracts (API contracts)
│   ├── Lunex.Domain/           # Domain layer (Entities, Business rules)
│   └── Lunex.Infrastructure/   # Infrastructure layer (DB, External services)
├── client/                     # Angular 20 Frontend
└── docs/                       # Documentation
```

## Layer Responsibilities

### 1. Domain Layer (`Lunex.Domain`)
**Core business logic - no external dependencies**

- **Entities**: `User`, `Member`, `Photo`
- **Value Objects**: None currently
- **Domain Events**: None currently
- **Domain Services**: None currently
- **Settings**: `CorsSettings`

### 2. Contracts Layer (`Lunex.Contracts`)
**Shared API contracts - no business logic**

- **Account Requests**: `RegisterUserRequest`, `LoginUserRequest`
- **Account Responses**: `UserResponse`
- **Member Requests**: `MemberRequest`, `MemberUpdateRequest`
- **Member Responses**: `MemberResponse`, `PhotoResponse`, `PhotoUploadResult`

### 3. Application Layer (`Lunex.Application`)
**Use cases and business logic orchestration**

- **Services**: `IAccountService`, `IMemberService`, `IJwtTokenGenerator`, `IPhotoService`, `IDateTimeProvider`
- **Repositories (Abstractions)**: `IAccountRepository`, `IMemberRepository`
- **Mappers**: `MemberMapper` (Entity ↔ DTO conversion)
- **Configurations**: `JwtSettings`, `CloudinarySettings`
- **Middleware**: `JwtBearerTokenValidationConfiguration`

### 4. Infrastructure Layer (`Lunex.Infrastructure`)
**External concerns implementation**

- **Persistence**: `ApplicationDbContext`, EF Core repositories
- **Migrations**: Database schema management
- **Services**: `CloudinaryService` (photo storage)
- **Seeding**: `DatabaseSeeder` (development data)

### 5. API Layer (`Lunex.Api`)
**HTTP endpoints and presentation concerns**

- **Controllers**: `AccountController`, `MembersController`
- **Extensions**: `AccountExtensions`, `ClaimsPrincipalExtensions`
- **Dependency Injection**: `DependencyInjection` (Web API setup)
- **Exception Handling**: `GlobalExceptionHandler`
- **Program.cs**: Application entry point, middleware pipeline

## Dependency Flow

```
┌─────────────────────────────────────────────────────────────┐
│                        Lunex.Api                            │
│  Controllers, Middleware, DI Configuration                  │
└──────────────────────────┬──────────────────────────────────┘
                           │ depends on
┌──────────────────────────▼──────────────────────────────────┐
│                     Lunex.Application                       │
│  Services, Repository Abstractions, Mappers, Config         │
└──────────────────────────┬──────────────────────────────────┘
                           │ depends on
┌──────────────────────────▼──────────────────────────────────┐
│                      Lunex.Domain                           │
│  Entities, Domain Logic, Settings                           │
└─────────────────────────────────────────────────────────────┘
                           ▲
                           │ implements
┌──────────────────────────┴──────────────────────────────────┐
│                   Lunex.Infrastructure                      │
│  EF Core, Repositories, Cloudinary, Seeding                 │
└─────────────────────────────────────────────────────────────┘
```

**Key Rule**: Inner layers never depend on outer layers. Dependencies point inward.

## Data Flow Example: Member Registration

```
1. HTTP POST /api/account/register
         │
         ▼
2. AccountController.Register()
   - Validates request (model binding)
   - Maps RegisterUserRequest → User entity (AccountExtensions.FromDto)
         │
         ▼
3. IAccountService.RegisterAsync()
   - Checks email uniqueness (IAccountRepository.EmailExistsAsync)
   - Persists User + Member (IAccountRepository.RegisterAsync)
         │
         ▼
4. AccountRepository.RegisterAsync()
   - EF Core: db.Users.AddAsync(), db.SaveChangesAsync()
         │
         ▼
5. AccountController returns UserResponse
   - Maps User → UserResponse (AccountExtensions.ToDto)
   - Generates JWT token (IJwtTokenGenerator.GenerateToken)
```

## Key Design Patterns

### Repository Pattern
- Abstractions in Application (`IAccountRepository`, `IMemberRepository`)
- Implementations in Infrastructure (`AccountRepository`, `MemberRepository`)
- Enables testing with in-memory/mock repositories

### Service Layer
- Business logic in Application services (`AccountService`, `MemberService`)
- Controllers are thin - only HTTP concerns

### Mapper Pattern
- Extension methods for Entity ↔ DTO conversion
- `MemberMapper.ToResponse()`, `AccountExtensions.ToDto()`

### Dependency Injection
- Scoped: Services, Repositories (per request)
- Singleton: `IDateTimeProvider`, `IJwtTokenGenerator`, `IPhotoService`
- Configured in each layer's `DependencyInjection.cs`

## Database Schema

**Schema**: `lunex` (PostgreSQL)

```
┌─────────────┐       ┌─────────────┐       ┌─────────────┐
│    users    │       │   members   │       │   photos    │
├─────────────┤       ├─────────────┤       ├─────────────┤
│ id (PK)     │◄──────│ id (PK, FK) │◄──────│ id (PK)     │
│ name        │       │ date_of_birth│       │ url         │
│ email       │       │ image_url   │       │ public_id   │
│ image_url   │       │ name        │       │ member_id(FK)│
│ password_hash│      │ gender      │       │ user_id(FK) │
│ password_salt│      │ description │       └─────────────┘
└─────────────┘       │ city        │
                      │ country     │
                      │ created_at  │
                      │ last_active │
                      └─────────────┘
```

## Authentication & Authorization

- **JWT Bearer Tokens** (HS512)
- **Token Expiry**: 30 minutes (configurable)
- **Claims**: `sub` (userId), `email`, `name`, `jti`, `nameid`
- **Authorization**: `[Authorize]` on `ApiController` base class
- **Anonymous**: `[AllowAnonymous]` on `AccountController`

## External Integrations

### Cloudinary (Photo Storage)
- Folder structure: `members/{memberId}/`
- Transformation: 500x500, fill crop, face gravity
- Secure URLs only

### PostgreSQL (Database)
- Provider: Npgsql
- Naming: Snake_case via `UseSnakeCaseNamingConvention()`
- Migrations: EF Core in `Lunex.Infrastructure`

## Frontend (Angular 20)

- **Location**: `client/`
- **Standalone Components** (no NgModules)
- **TailwindCSS + DaisyUI** for styling
- **Port**: 4200 (dev), proxied to API on 5001
- **HTTP Client**: Angular's `HttpClient` with interceptors

## Development vs Production

| Aspect | Development | Production |
|--------|-------------|------------|
| OpenAPI/Scalar | Enabled (`/scalar/v1`) | Disabled |
| Database Seeding | Auto on startup | Manual |
| CORS | localhost:4200 | Configured domains |
| HTTPS | localhost:5001 | Real certificates |
| Logging | Debug/Information | Warning/Error |

## Scalability Considerations

- **Stateless API** - horizontal scaling ready
- **EF Core** - connection pooling configured
- **Cloudinary** - CDN for photo delivery
- **JWT** - no server-side session storage
- **Caching** - not yet implemented (planned: Redis)