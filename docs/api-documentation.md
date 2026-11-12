# Lunex API Documentation

## Overview

The Lunex API provides RESTful endpoints for user account management and member operations. All endpoints return JSON responses and use standard HTTP status codes.

**Base URL:** `https://api.lunex.com/api` (adjust for your environment)

**API Documentation UI:** Available at `/scalar/v1` when running in Development environment (Scalar OpenAPI viewer)

---

## Authentication

Most endpoints require authentication via JWT Bearer token. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

Tokens are obtained via the `/api/account/login` or `/api/account/register` endpoints.

### JWT Token Details

- **Algorithm:** HS512 (HMAC SHA-512)
- **Expiry:** Configurable via `JwtSettings:ExpiryMinutes` (default: 60 minutes)
- **Claims:** Contains user ID, email, and name

---

## Account Endpoints

### Register User

Creates a new user account and returns a JWT token for authentication.

**Endpoint:** `POST /api/account/register`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

| Field | Type | Required | Validation Rules | Description |
|-------|------|----------|------------------|-------------|
| name | string | Yes | Min: 1, Max: 100 chars | User's full name |
| email | string | Yes | Valid email format, Max: 255 chars, Unique | User's email address |
| password | string | Yes | Min: 8 chars, Max: 100 chars, Must contain: uppercase, lowercase, number, special char | User's password |

**Response (200 OK):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "email": "john.doe@example.com",
  "name": "John Doe",
  "imageUrl": "",
  "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses:**

| Status | Condition | Response Body |
|--------|-----------|---------------|
| 400 | Email already exists | `{"message": "Email exists"}` |
| 400 | Validation failed | `{"type": "https://tools.ietf.org/html/rfc7231#section-6.5.1", "title": "One or more validation errors occurred.", "status": 400, "errors": {"email": ["The email field is required."]}}` |

**cURL Example:**
```bash
curl -X POST https://api.lunex.com/api/account/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "password": "SecurePass123!"
  }'
```

---

### Login User

Authenticates a user and returns a JWT token.

**Endpoint:** `POST /api/account/login`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | string | Yes | User's registered email address |
| password | string | Yes | User's password |

**Response (200 OK):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "email": "john.doe@example.com",
  "name": "John Doe",
  "imageUrl": "",
  "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses:**

| Status | Condition | Response Body |
|--------|-----------|---------------|
| 401 | Invalid email | `{"message": "Invalid email address"}` |
| 401 | Invalid password | `{"message": "Invalid password"}` |

**cURL Example:**
```bash
curl -X POST https://api.lunex.com/api/account/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePass123!"
  }'
```

---

## Member Endpoints

### Get All Members

Retrieves a list of all members. Requires authentication.

**Endpoint:** `GET /api/members`

**Headers:**
```
Authorization: Bearer <token>
Content-Type: application/json
```

**Query Parameters (Planned):**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number for pagination |
| pageSize | integer | 20 | Number of items per page (max: 100) |
| search | string | - | Search by name or email |
| sortBy | string | createdAt | Sort field (createdAt, name, email) |
| sortOrder | string | desc | Sort order (asc, desc) |

**Response (200 OK):**
```json
[
  {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "dateOfBirth": "1990-05-15",
    "imageUrl": "https://example.com/avatar.jpg",
    "name": "Jane Smith",
    "gender": "Female",
    "description": "Software developer passionate about AI",
    "city": "San Francisco",
    "country": "USA",
    "createdAt": "2026-01-15T10:30:00Z",
    "lastActiveAt": "2026-02-09T14:22:00Z"
  },
  {
    "id": "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    "dateOfBirth": "1985-12-22",
    "imageUrl": "",
    "name": "Bob Wilson",
    "gender": "Male",
    "description": "Full-stack engineer",
    "city": "New York",
    "country": "USA",
    "createdAt": "2026-01-20T08:15:00Z",
    "lastActiveAt": "2026-02-08T16:45:00Z"
  }
]
```

**Error Responses:**

| Status | Condition | Response Body |
|--------|-----------|---------------|
| 401 | Missing/invalid token | `{"type": "https://tools.ietf.org/html/rfc7235", "title": "Unauthorized", "status": 401}` |

**cURL Example:**
```bash
curl -X GET "https://api.lunex.com/api/members?page=1&pageSize=10&sortBy=name&sortOrder=asc" \
  -H "Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
```

---

### Get Member by ID

Retrieves a specific member by their ID. Requires authentication.

**Endpoint:** `GET /api/members/{id}`

**Headers:**
```
Authorization: Bearer <token>
```

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | string | Member's unique identifier (GUID format) |

**Response (200 OK):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "dateOfBirth": "1990-05-15",
  "imageUrl": "https://example.com/avatar.jpg",
  "name": "Jane Smith",
  "gender": "Female",
  "description": "Software developer passionate about AI",
  "city": "San Francisco",
  "country": "USA",
  "createdAt": "2026-01-15T10:30:00Z",
  "lastActiveAt": "2026-02-09T14:22:00Z"
}
```

**Error Responses:**

| Status | Condition | Response Body |
|--------|-----------|---------------|
| 401 | Missing/invalid token | `{"type": "https://tools.ietf.org/html/rfc7235", "title": "Unauthorized", "status": 401}` |
| 404 | Member not found | `{"type": "https://tools.ietf.org/html/rfc7231#section-6.5.4", "title": "Not Found", "status": 404}` |

**cURL Example:**
```bash
curl -X GET https://api.lunex.com/api/members/a1b2c3d4-e5f6-7890-abcd-ef1234567890 \
  -H "Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9..."
```

---

## Data Models

### UserResponse

Returned by authentication endpoints.

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | string | No | Unique user identifier (GUID) |
| email | string | No | User's email address |
| name | string | No | User's full name |
| imageUrl | string | No | Profile image URL (empty string if not set) |
| token | string | No | JWT authentication token |

### RegisterUserRequest

| Field | Type | Nullable | Validation |
|-------|------|----------|------------|
| name | string | No | Required, 1-100 chars |
| email | string | No | Required, valid email, unique |
| password | string | No | Required, 8-100 chars, complexity requirements |

### LoginUserRequest

| Field | Type | Nullable | Validation |
|-------|------|----------|------------|
| email | string | No | Required, valid email |
| password | string | No | Required |

### Member (User with Member Profile)

| Field | Type | Nullable | Description |
|-------|------|----------|-------------|
| id | string | No | Unique identifier (GUID) |
| dateOfBirth | date | No | Member's date of birth (ISO 8601) |
| imageUrl | string | Yes | Profile image URL |
| name | string | No | Member's full name |
| gender | string | No | Member's gender |
| description | string | No | Member's description/bio |
| city | string | No | Member's city |
| country | string | No | Member's country |
| createdAt | datetime | No | Timestamp when member was created (ISO 8601 UTC) |
| lastActiveAt | datetime | No | Timestamp of last activity (ISO 8601 UTC) |

---

## HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request - Invalid input or validation failed |
| 401 | Unauthorized - Missing or invalid authentication |
| 404 | Not Found - Resource not found |
| 500 | Internal Server Error |

---

## Error Response Format

The API uses RFC 7807 Problem Details for error responses.

### Validation Error (400)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "email": ["The email field is required.", "Invalid email format."],
    "password": ["Password must be at least 8 characters."]
  }
}
```

### Unauthorized (401)
```json
{
  "type": "https://tools.ietf.org/html/rfc7235",
  "title": "Unauthorized",
  "status": 401
}
```

### Not Found (404)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404
}
```

### Server Error (500)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Server Error",
  "status": 500
}
```

---

## Rate Limiting

**Current Status:** Not implemented

**Planned Implementation:**
- **Anonymous endpoints** (register, login): 10 requests/minute per IP
- **Authenticated endpoints**: 100 requests/minute per user
- **Headers returned:**
  - `X-RateLimit-Limit`: Request limit
  - `X-RateLimit-Remaining`: Remaining requests
  - `X-RateLimit-Reset`: Unix timestamp when limit resets

---

## API Documentation (Swagger/OpenAPI)

The API uses **Scalar** as the OpenAPI documentation viewer (modern alternative to Swagger UI).

### Accessing Documentation

**Development Environment:**
- URL: `https://localhost:<port>/scalar/v1`
- OpenAPI JSON: `https://localhost:<port>/openapi/v1.json`

**Production:** Documentation UI is disabled in non-Development environments for security.

### Configuration

The OpenAPI specification is generated automatically from:
- Controller action attributes (`[HttpGet]`, `[HttpPost]`, etc.)
- Route attributes (`[Route("api/account")]`)
- Parameter attributes (`[FromBody]`, `[FromRoute]`, `[FromQuery]`)
- XML comments (if enabled)

### Adding XML Comments

To enrich the OpenAPI spec with descriptions, add XML documentation to controllers:

```csharp
/// <summary>
/// Registers a new user account
/// </summary>
/// <param name="request">Registration details</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>User details with JWT token</returns>
/// <response code="200">Registration successful</response>
/// <response code="400">Email already exists or validation failed</response>
[HttpPost("register")]
[ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Register(...)
```

---

## CORS Configuration

The API supports Cross-Origin Resource Sharing (CORS) with the following settings:

- **Allowed Origins:** Configured via `CorsSettings:AllowedOrigins` in appsettings.json
- **Allowed Headers:** All (`AllowAnyHeader()`)
- **Allowed Methods:** All (`AllowAnyMethod()`)
- **Credentials:** Allowed (`AllowCredentials()`)

Example configuration:
```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:4200",
      "https://app.lunex.com"
    ]
  }
}
```

---

## Example Usage Workflows

### Complete Authentication Flow

```bash
# 1. Register a new user
REGISTER_RESPONSE=$(curl -s -X POST https://api.lunex.com/api/account/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "password": "SecurePass123!"
  }')

# Extract token (requires jq)
TOKEN=$(echo $REGISTER_RESPONSE | jq -r '.token')

# 2. Use token to get members
curl -X GET https://api.lunex.com/api/members \
  -H "Authorization: Bearer $TOKEN"

# 3. Get specific member
curl -X GET https://api.lunex.com/api/members/a1b2c3d4-e5f6-7890-abcd-ef1234567890 \
  -H "Authorization: Bearer $TOKEN"
```

### Login Flow

```bash
# Login
LOGIN_RESPONSE=$(curl -s -X POST https://api.lunex.com/api/account/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePass123!"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.token')

# Use token for authenticated requests
curl -X GET https://api.lunex.com/api/members \
  -H "Authorization: Bearer $TOKEN"
```

---

## Version

Current API Version: **v1**

Versioning Strategy: URL path versioning planned (`/api/v1/...`)

---

## Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2026-02-09 | Initial API release with account (register, login) and member (list, get by ID) endpoints |
| 1.1.0 | 2026-02-12 | Added comprehensive API documentation with examples, validation rules, error formats, and Scalar/OpenAPI integration details |

---

## Future Endpoints (Planned)

The following endpoints are planned for future releases:

### Member Management
- `POST /api/members` - Create member profile
- `PUT /api/members/{id}` - Update member profile
- `DELETE /api/members/{id}` - Delete member profile
- `POST /api/members/{id}/avatar` - Upload profile image

### Account Management
- `PUT /api/account/password` - Change password
- `PUT /api/account/profile` - Update profile (name, image)
- `POST /api/account/refresh` - Refresh JWT token
- `POST /api/account/logout` - Invalidate token (blocklist)

### Search & Discovery
- `GET /api/members/search` - Advanced member search with filters
- `GET /api/members/nearby` - Location-based member discovery