# Contracts Layer Design Pattern

## Why Contracts Cannot Reference Application/Infrastructure

The **Contracts** layer must be a **standalone, independent package** that can be published as a NuGet package for external clients to consume.

### Dependency Rules

```
✅ ALLOWED:
  - API references Contracts ✓
  - API references Application ✓
  - Application references Domain ✓
  - Infrastructure references Application & Domain ✓
  - Contracts reference nothing ✓

❌ NOT ALLOWED:
  - Application references Contracts ✗
  - Infrastructure references Contracts ✗
  - Domain references Contracts ✗
  - Contracts reference anything ✗
```

## Project References

```
Lunex.Api.csproj
├── → Lunex.Application
├── → Lunex.Contracts
└── → Lunex.Infrastructure

Lunex.Application.csproj
└── → Lunex.Domain

Lunex.Infrastructure.csproj
├── → Lunex.Application
└── → Lunex.Domain

Lunex.Contracts.csproj
└── (no references)

Lunex.Domain.csproj
└── (no references)
```

## Data Flow: How Contracts DTOs are Used

### 1. API Receives HTTP Request (Contracts DTO)
```csharp
// MembersController.cs (API Layer)
[HttpPost("profile")]
public async Task<IActionResult> CreateProfile(
    [FromBody] MemberCreateRequest request,  // ← Contracts DTO
    CancellationToken cancellationToken)
{
    var userId = User.GetRequiredUserId();

    // Map Contracts → Primitives for Application layer
    var member = await _memberService.CreateProfileAsync(
        userId,
        request.Name,                    // string
        request.Gender,                  // string
        request.DateOfBirth,             // DateOnly
        request.City,                    // string
        request.Country,                 // string
        request.Description,             // string?
        cancellationToken);

    // Map Application DTOs → Contracts Response DTO
    var response = new MemberResponse(
        member.Id,
        member.Name,
        member.Gender.ToDisplayString(),
        member.Description,
        member.Location.City,
        member.Location.Country,
        member.ImageUrl,
        member.DateOfBirth.Value,
        member.CreatedAt,
        member.LastActiveAt,
        member.Photos.Select(p => new PhotoResponse(
            p.Id, p.Url, p.PublicId)).ToList()
    );

    return CreatedAtAction(nameof(GetById), new { id = member.Id }, response);
}
```

### 2. Application Layer Processes Request (Application DTOs)
```csharp
// IMemberService.cs (Application Layer)
Task<MemberDto> CreateProfileAsync(
    string userId,
    string name,
    string gender,
    DateOnly dateOfBirth,
    string city,
    string country,
    string? description,
    CancellationToken cancellationToken);
```

### 3. Domain Aggregates Store Business Logic (Value Objects)
```csharp
// Member.cs (Domain Layer)
public sealed class Member : IAggregateRoot
{
    public string Id { get; }
    public string UserId { get; }
    public string Name { get; }
    public Gender Gender { get; }           // ← Value Object
    public DateOfBirth DateOfBirth { get; } // ← Value Object
    public Location Location { get; }       // ← Value Object (City/Country)
    public string Description { get; }

    public static Member CreateProfile(
        string userId,
        string name,
        string genderValue,
        DateOnly dateOfBirth,
        string city,
        string country,
        string? description)
    {
        // Validation and conversion happen here
        var gender = Gender.FromString(genderValue);
        var dob = DateOfBirth.From(dateOfBirth);
        var location = new Location(city, country);

        var member = new Member { /* ... */ };
        member.RaiseDomainEvent(new MemberProfileCreated(/* ... */));
        return member;
    }
}
```

### 4. API Returns HTTP Response (Contracts DTO)
```
HTTP 201 Created
{
  "id": "member-123",
  "name": "John Doe",
  "gender": "Male",
  "dateOfBirth": "1990-05-15",
  "city": "New York",
  "country": "USA",
  "description": "...",
  "createdAt": "2026-11-20T01:43:05Z",
  "lastActiveAt": "2026-11-20T01:43:05Z",
  "photos": []
}
```

## Mapping Responsibilities by Layer

| Layer | Input Type | Output Type | Mapping Logic |
|-------|-----------|------------|---------------|
| **API** | Contracts DTO | Contracts DTO | Receives Contracts, returns Contracts |
| **API→App** | Contracts props | Primitives | Extract properties from Contracts |
| **Application** | Primitives | App DTOs | Maps Domain aggregates to DTOs |
| **App→Domain** | Primitives | Domain commands | Passes primitives to domain methods |
| **Domain** | Primitives | Aggregates | Converts to Value Objects, enforces rules |

## Example: Register User Flow

```
Step 1: API Layer receives Contracts.RegisterUserRequest
┌─────────────────────────────────────────────────────┐
│ POST /api/account/register                          │
│ {                                                   │
│   "email": "user@example.com",                      │
│   "password": "SecurePassword123",                  │
│   "name": "John Doe"                                │
│ }                                                   │
└─────────────────────────────────────────────────────┘
            ↓ (map to Application DTO)

Step 2: Application layer processes primitives
┌─────────────────────────────────────────────────────┐
│ IdentityService.RegisterAsync(                      │
│   email: "user@example.com",                        │
│   password: "SecurePassword123",                    │
│   name: "John Doe")                                 │
└─────────────────────────────────────────────────────┘
            ↓ (call domain method)

Step 3: Domain layer creates aggregate
┌─────────────────────────────────────────────────────┐
│ User.Register(                                      │
│   email: Email.Create("user@example.com"),          │
│   password: "SecurePassword123",                    │
│   name: "John Doe")                                 │
│                                                     │
│ Raises: UserRegistered event                        │
│ Returns: User aggregate with Id, Email VO, etc.     │
└─────────────────────────────────────────────────────┘
            ↓ (persist & map back)

Step 4: API layer returns Contracts.UserResponse
┌─────────────────────────────────────────────────────┐
│ HTTP 200 OK                                         │
│ {                                                   │
│   "id": "user-123",                                 │
│   "email": "user@example.com",                      │
│   "name": "John Doe",                               │
│   "imageUrl": "",                                   │
│   "token": "eyJhbGc..."                             │
│ }                                                   │
└─────────────────────────────────────────────────────┘
```

## Why This Matters

### 1. **Contracts as NuGet Package**
```csharp
// client/Program.cs
using Lunex.Contracts.Accounts;
using Lunex.Contracts.Members;

// Clients only need Contracts, no dependency on internal layers!
var client = new HttpClient { BaseAddress = new("https://api.example.com") };
var registerRequest = new RegisterUserRequest(
    Email: "user@example.com",
    Password: "password",
    Name: "John");

var response = await client.PostAsJsonAsync("/api/account/register", registerRequest);
var userResponse = await response.Content.ReadAsAsync<UserResponse>();
```

### 2. **Clean Architecture**
- Domain stays pure business logic
- Application orchestrates use cases
- Infrastructure implements technical concerns
- API coordinates HTTP
- Contracts remain stable, versioned separately

### 3. **Easy Evolution**
- Change internal implementation without breaking API clients
- Application/Infrastructure can be refactored safely
- Contracts only change when API contract changes

### 4. **Testability**
- Domain unit tests: no external dependencies
- Application tests: mock repositories
- API tests: mock services
- Contracts: just data structures

## Common Mistakes to Avoid

❌ **Wrong: Application referencing Contracts**
```csharp
// ❌ BAD - Application/Member/Services/MemberService.cs
using Lunex.Contracts.Members.Requests;  // ✗ Contracts in Application!

public Task<MemberResponse> GetMembersAsync(MemberParams @params)
{
    // Now Application depends on Contracts versioning
    // Can't publish Application independently
}
```

✅ **Right: Application using primitives**
```csharp
// ✅ GOOD - Application/Member/Services/MemberService.cs
public Task<PaginatedResult<MemberDto>> GetMembersAsync(
    int pageNumber,
    int pageSize,
    string? gender = null,
    string? currentMemberId = null,
    CancellationToken cancellationToken = default)
{
    // Uses primitives, Contracts handled only in API layer
}
```

---

**Rule of Thumb:** If you're in Application or Infrastructure layer and think about importing from Contracts, stop and ask: "Should this be a primitive type instead?"
