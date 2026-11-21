# Lunex API - Domain-Driven Design Architecture

## Overview

The Lunex API has been refactored following **Domain-Driven Design (DDD)** principles with clear separation of concerns across multiple layers and bounded contexts.

## Architecture Layers

### 1. **Domain Layer** (`Lunex.Domain`)
The core business logic layer - no external dependencies.

**Bounded Contexts:**

#### Identity Context
- **Aggregates:**
  - `User` - Aggregate root representing authentication identity
    - Behavior: `Register()`, `VerifyPassword()`, `ChangePassword()`, `RecordLogin()`, `UpdateProfile()`
    - Domain Events: `UserRegistered`, `UserLoggedIn`

- **Value Objects:**
  - `Email` - Validated email value object
  - `PasswordHash` - Encrypted password representation
  - `PasswordSalt` - Password salt for encryption

- **Repository Interface:**
  - `IUserRepository` - Contract for user persistence

- **Domain Service:**
  - `IPasswordHasher` - Cross-cutting password hashing service

#### Member Context
- **Aggregates:**
  - `Member` - Aggregate root representing member profile
    - Behavior: `CreateProfile()`, `UpdateProfile()`, `AddPhoto()`, `RemovePhoto()`, `SetMainPhoto()`, `UpdateActivity()`
    - Entities: `Photo` (child entity, only accessible through Member)
    - Domain Events: `MemberProfileCreated`, `MemberProfileUpdated`, `PhotoAdded`, `PhotoRemoved`, `MemberActivityUpdated`

- **Value Objects:**
  - `Gender` - Enum with display string conversion
  - `Location` - Immutable city/country value object
  - `DateOfBirth` - Validated date of birth with age calculation

- **Repository Interface:**
  - `IMemberRepository` - Contract for member persistence

#### Shared Kernel (Common)
- `IDomainEvent` - Domain event interface
- `DomainEventBase` - Base record for all domain events
- `IAggregateRoot` - Interface marking aggregate roots with domain events
- `PaginationParams`, `PaginatedResult` - Cross-context pagination

### 2. **Application Layer** (`Lunex.Application`)
Thin orchestration layer that uses domain models and coordinates application use cases.

**Dependencies:** Domain only (no Contracts or Infrastructure)

**Structure:**

#### Identity Application Service
- **Commands:**
  - `RegisterUserCommand` - Request → `TokenResponse`
  - `LoginUserCommand` - Request → `TokenResponse`
  - `ChangePasswordCommand` - Request → `bool`

- **Queries:**
  - `GetUserByIdQuery` - Request → `UserDto`
  - `GetUserByEmailQuery` - Request → `UserDto`

- **Handlers:** MediatR handlers for each command/query
  - Orchestrate domain logic
  - Map domain → DTOs
  - Publish domain events

- **Service:** `IIdentityService` - Facade for identity operations
  - `RegisterAsync()`, `LoginAsync()`, `GetUserAsync()`
  - Domain-agnostic, works with aggregates

- **DTOs:**
  - `RegisterUserRequest` - Primitive types (string, int)
  - `LoginUserRequest` - Primitive types
  - `TokenResponse` - Primitive types
  - `UserDto` - Mapper for domain → response (internal)

#### Member Application Service
- **Commands:**
  - `CreateMemberProfileCommand` - Create profile
  - `UpdateMemberProfileCommand` - Update profile
  - `AddPhotoCommand` - Add photo to profile
  - `RemovePhotoCommand` - Remove photo
  - `SetMainPhotoCommand` - Set main photo

- **Queries:**
  - `GetMemberByIdQuery` - Get by ID
  - `GetMemberByUserIdQuery` - Get by user ID
  - `GetMembersQuery` - Paginated list with filtering

- **Handlers:** MediatR handlers
  - Load aggregates from repositories
  - Execute domain behavior
  - Persist changes
  - Publish events

- **Service:** `IMemberService` - Facade for member operations
  - `CreateProfileAsync()`, `UpdateProfileAsync()`
  - `AddPhotoAsync()`, `RemovePhotoAsync()`, `SetMainPhotoAsync()`
  - `GetAsync()` - Paginated with filtering

- **DTOs:**
  - `MemberDto`, `MemberProfileDto`, `PhotoDto` - Internal mappers
  - Primitive type conversions from Value Objects

#### Common Services
- `IDateTimeProvider` - Abstraction for current time (testability)
- `IJwtTokenGenerator` - JWT token generation
- `IPhotoService` - Photo upload/deletion abstraction
- `PhotoUploadResult` - Upload result model

### 3. **Infrastructure Layer** (`Lunex.Infrastructure`)
Implements technical concerns and repositories.

**Dependencies:** Application, Domain

**Key Components:**

- **Repositories:**
  - `UserRepository` - Implements `IUserRepository`
  - `MemberRepository` - Implements `IMemberRepository` with filtering/pagination

- **Database:**
  - `ApplicationDbContext` - EF Core DbContext
  - ValueConverters for Value Objects (Email, PasswordHash, etc.)
  - Automatic mapping between domain → database

- **Services:**
  - `PhotoService` - Cloudinary integration
  - `PasswordHasher` - HMACSHA512 password hashing
  - `JwtTokenGenerator` - JWT token creation
  - `DateTimeProvider` - Provides current UTC time

- **Seeding:**
  - `DatabaseSeeder` - Initializes test data

### 4. **API Layer** (`Lunex.Api`)
HTTP endpoints and request/response handling.

**Dependencies:** Application, Contracts, Infrastructure

**Key Components:**

- **Controllers:**
  - `AccountController` - Register, Login endpoints
  - `MembersController` - Profile CRUD, photo management

- **Mapping:**
  - Contracts DTOs → Application DTOs
  - Application DTOs → Contracts responses
  - Value Objects → Primitive strings/enums

- **Filters:**
  - `LogUserActivity` - Updates member LastActiveAt on each request

- **Global Exception Handling:**
  - `GlobalExceptionHandler` - Centralized error responses

### 5. **Contracts Layer** (`Lunex.Contracts`)
API contract definitions - publishable as NuGet package.

**Independence:** No dependencies on other projects

**Structure:**
```
Contracts/
├── Accounts/
│   ├── Register/Requests/RegisterUserRequest
│   ├── Login/Requests/LoginUserRequest
│   └── Common/Responses/UserResponse
└── Members/
    ├── Requests/MemberCreateRequest, MemberUpdateRequest
    └── Responses/MemberResponse, PhotoResponse, PhotoUploadResult
```

## Dependency Flow

```
API Layer
  ↓ depends on
Application Layer  ← Contracts (API only)
  ↓ depends on
Domain Layer
  ↓ depends on
(nothing - pure business logic)

Infrastructure
  ↓ depends on
Application + Domain

Contracts
  ↓ depends on
(nothing - publishable NuGet)
```

**Key Rule:** Contracts should never be referenced by Application or Infrastructure - they are API contracts only!

## Domain-Driven Design Principles Applied

### 1. **Ubiquitous Language**
- Domain concepts: User, Member, Photo, Email, Gender, Location
- Domain events: UserRegistered, MemberProfileCreated, PhotoAdded
- Shared understanding between developers and domain experts

### 2. **Bounded Contexts**
- **Identity Context**: Handles user authentication only
- **Member Context**: Handles member profiles and photos
- Clear separation with explicit boundaries

### 3. **Aggregates**
- **User Aggregate**: User is root, contains no nested aggregates
- **Member Aggregate**: Member is root, contains Photo entities
- Consistency enforced at aggregate level

### 4. **Value Objects**
- Immutable: Email, PasswordHash, Gender, Location, DateOfBirth
- Validation: Email format, password requirements, age calculation
- No identity: Compared by value, not reference

### 5. **Domain Events**
- Represent business-important changes: UserRegistered, MemberProfileCreated
- Published by aggregates when state changes
- Subscribed by handlers for side effects (email notifications, logging)

### 6. **Repository Pattern**
- One repository per aggregate root (IUserRepository, IMemberRepository)
- Abstracts persistence mechanism
- Returns aggregates, not DTOs
- Domain-focused queries

### 7. **Domain Services**
- Cross-aggregate logic: IPasswordHasher, IPhotoService
- Injected into application services
- Domain knowledge, not technical details

### 8. **Application Services**
- Thin orchestration layer
- Loads aggregates from repositories
- Executes domain methods
- Publishes domain events
- Maps to DTOs for API responses

## Data Flow Example: Register User

```
1. API receives POST /api/account/register
   ↓
2. Controller maps Contracts.RegisterUserRequest → Application.RegisterUserRequest
   ↓
3. MediatR dispatches RegisterUserCommand(request)
   ↓
4. RegisterUserCommandHandler:
   - Validates email doesn't exist (IUserRepository.GetByEmailAsync)
   - Calls User.Register() [domain behavior]
     * Hashes password via IPasswordHasher
     * Creates User aggregate
     * Raises UserRegistered event
   - Saves User via IUserRepository.AddAsync()
   - Generates JWT via IJwtTokenGenerator
   - Maps User → UserDto → UserResponse
   ↓
5. Controller returns 200 OK with UserResponse
```

## Data Flow Example: Create Member Profile

```
1. API receives POST /api/members/profile [Authorize]
   ↓
2. Controller maps Contracts.MemberCreateRequest → command params
   ↓
3. MediatR dispatches CreateMemberProfileCommand
   ↓
4. CreateMemberProfileCommandHandler:
   - Loads User from IUserRepository
   - Calls Member.CreateProfile() [domain behavior]
     * Validates DateOfBirth (via DateOfBirth value object)
     * Creates Location value object
     * Creates Gender value object
     * Returns new Member aggregate
     * Raises MemberProfileCreated event
   - Saves Member via IMemberRepository.AddAsync()
   - Maps Member → MemberDto → MemberResponse
   ↓
5. Controller returns 201 Created with MemberResponse
```

## Testing Strategy

### Unit Tests (Domain)
- Test aggregate behavior: `User.Register()`, `Member.AddPhoto()`
- Test value object validation: Email, DateOfBirth
- Test domain events are raised correctly

### Integration Tests (Application)
- Test command/query handlers orchestration
- Test repository contracts
- Mock infrastructure services (IPhotoService)

### API Tests (End-to-End)
- Test HTTP endpoints
- Test request/response mapping
- Test authorization and filtering

## Future Extensions

1. **Event Sourcing**
   - Store domain events as source of truth
   - Replay events to rebuild state

2. **CQRS Separation**
   - Separate write models (Commands) from read models (Queries)
   - Optimize read performance with projections

3. **Process Managers**
   - Coordinate long-running business processes across aggregates
   - Handle saga pattern for distributed transactions

4. **Anti-Corruption Layer**
   - Translate between bounded contexts
   - Isolate from external systems

## File Structure Summary

```
src/
├── Lunex.Domain/
│   ├── Common/                    # Shared kernel
│   ├── Identity/                  # Authentication bounded context
│   │   ├── Aggregates/User.cs
│   │   ├── ValueObjects/Email.cs, PasswordHash.cs, etc.
│   │   ├── Events/UserRegistered.cs, etc.
│   │   ├── Repositories/IUserRepository.cs
│   │   └── Services/IPasswordHasher.cs
│   └── Member/                    # Profile bounded context
│       ├── Aggregates/Member.cs, Photo.cs
│       ├── ValueObjects/Gender.cs, Location.cs, etc.
│       ├── Events/MemberProfileCreated.cs, etc.
│       └── Repositories/IMemberRepository.cs
│
├── Lunex.Application/             # Application services (no Contracts!)
│   ├── Common/Services/           # DTOs and cross-cutting concerns
│   ├── Identity/
│   │   ├── Commands/, Queries/, Handlers/
│   │   ├── DTOs/
│   │   └── Services/IIdentityService.cs
│   └── Member/
│       ├── Commands/, Queries/, Handlers/
│       ├── DTOs/
│       └── Services/IMemberService.cs
│
├── Lunex.Infrastructure/          # Technical implementation
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   └── Repositories/
│   ├── Services/
│   └── Seeder/
│
├── Lunex.Api/                     # HTTP layer
│   ├── Controllers/
│   ├── Filters/
│   ├── Extensions/
│   └── Program.cs
│
└── Lunex.Contracts/               # API contracts (standalone NuGet)
    ├── Accounts/
    └── Members/
```

---

**Architecture Decision Record:** This project uses Domain-Driven Design to enforce clear boundaries between business logic (domain), use cases (application), technical concerns (infrastructure), and API contracts. This enables:
- Testability: Domain and Application layers have no external dependencies
- Maintainability: Clear separation of concerns
- Scalability: Easy to add new bounded contexts
- Independence: Contracts can be published as a separate NuGet package for external clients
