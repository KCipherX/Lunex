# Lunex API Reference

This document describes the HTTP API currently implemented by Lunex. It documents implemented behavior only; planned endpoints and unsupported query parameters are omitted.

## Base URL and OpenAPI

Use the API origin for your environment. The local HTTP request examples in [`https/account.http`](../https/account.http), [`https/members.http`](../https/members.http), and [`https/photos.http`](../https/photos.http) use `https://localhost:5001`.

In Development, the API exposes an OpenAPI document at `/openapi/v1.json` and the Scalar UI at `/scalar/v1`. These routes are not mapped by the application outside Development.

All routes below are relative to the API origin. JSON request and response property names use camel case.

## Authentication

`POST /api/account/register` and `POST /api/account/login` allow anonymous requests. The members controller inherits `[Authorize]`; all `/api/members` routes require a valid bearer token.

For authenticated requests, send:

```http
Authorization: Bearer <token>
```

Registration and login return a token in the `token` property of the response. Send that token on subsequent member requests. A missing or invalid token is rejected by ASP.NET Core authorization middleware with `401 Unauthorized`.

## Endpoints

| Method | Route | Authentication | Description |
|---|---|---|---|
| `POST` | `/api/account/register` | Anonymous | Register an account and return its user data and token |
| `POST` | `/api/account/login` | Anonymous | Authenticate and return user data and token |
| `GET` | `/api/members` | Bearer token | List members |
| `GET` | `/api/members/{id}` | Bearer token | Get a member by ID |
| `GET` | `/api/members/{id}/photos` | Bearer token | List a member's photos |
| `PUT` | `/api/members` | Bearer token | Update the authenticated member's profile |
| `POST` | `/api/members/photos` | Bearer token | Upload a photo for the authenticated member |
| `DELETE` | `/api/members/photos/{photoId}` | Bearer token | Delete a photo belonging to the authenticated member |

### Register

`POST /api/account/register`

Content type: `application/json`.

```json
{
  "name": "Jordan Lee",
  "email": "jordan@example.com",
  "password": "ExamplePass123!",
  "gender": "Female",
  "city": "Toronto",
  "country": "Canada",
  "dateOfBirth": "1990-01-15",
  "description": "Software developer"
}
```

| Property | Type | Required | Constraints |
|---|---|---|---|
| `name` | string | Yes | 2?100 characters |
| `email` | string | Yes | Email address; at most 256 characters |
| `password` | string | Yes | 8?100 characters |
| `gender` | string | Yes | At most 20 characters |
| `city` | string | Yes | At most 100 characters |
| `country` | string | Yes | At most 100 characters |
| `dateOfBirth` | date string | Yes | `YYYY-MM-DD` (`DateOnly`) |
| `description` | string or null | No | At most 500 characters; defaults to null |

On success, returns `200 OK`:

```json
{
  "id": "member-user-id",
  "email": "jordan@example.com",
  "name": "Jordan Lee",
  "imageUrl": "",
  "token": "<jwt>"
}
```

If the email is already in use, returns `400 Bad Request` with the message `Email exists`. Invalid request fields are rejected by API model validation with `400 Bad Request`.

### Login

`POST /api/account/login`

Content type: `application/json`.

```json
{
  "email": "jordan@example.com",
  "password": "ExamplePass123!"
}
```

On success, returns `200 OK` with the same `UserResponse` shape as registration. An unrecognized email returns `401 Unauthorized` with `Invalid email address`; a password mismatch returns `401 Unauthorized` with `Invalid password`.

### List members

`GET /api/members`

Requires a bearer token. No pagination, search, or sorting query parameters are currently implemented. Returns `200 OK` with an array of `MemberResponse` objects; an empty array is possible.

### Get a member

`GET /api/members/{id}`

Requires a bearer token. `id` is the member identifier (string). Returns `200 OK` with a `MemberResponse`, or `404 Not Found` if no member with that ID exists.

### Get member photos

`GET /api/members/{id}/photos`

Requires a bearer token. Returns `200 OK` with an array of `PhotoResponse` objects for the member (which may be empty).

### Update the authenticated member

`PUT /api/members`

Requires a bearer token. The member ID is taken from the authenticated user's token; do not send an ID in the body. Content type: `application/json`.

```json
{
  "displayName": "Jordan Lee",
  "description": "Updated profile description",
  "city": "Toronto",
  "country": "Canada"
}
```

All four properties are strings. Returns `200 OK` with the updated `MemberResponse`, or `404 Not Found` if the authenticated user has no member profile.

### Upload a photo

`POST /api/members/photos`

Requires a bearer token. Send `multipart/form-data` with a file part named `file`. The request limit is 10,000,000 bytes (10 MB). The filename extension must be `.jpg`, `.jpeg`, `.png`, `.gif`, or `.webp` (case-insensitive).

Returns `200 OK` with the stored photo:

```json
{
  "id": "photo-id",
  "url": "https://example.invalid/photo.jpg",
  "publicId": "members/member-id/photo-id"
}
```

An empty or missing file returns `400 Bad Request` (`No file uploaded`); an unsupported extension returns `400 Bad Request` (`Invalid file type. Allowed: jpg, jpeg, png, gif, webp`). Actual image storage is handled by the configured photo provider.

### Delete a photo

`DELETE /api/members/photos/{photoId}`

Requires a bearer token. A member can delete only a photo associated with their own member profile. Returns `204 No Content` when deleted, or `404 Not Found` when the photo is not found for that member.

## Response models

### `UserResponse`

| Property | Type | Notes |
|---|---|---|
| `id` | string | User identifier |
| `email` | string | Email address |
| `name` | string | Display name |
| `imageUrl` | string | Currently returned as an empty string by account responses |
| `token` | string | JWT bearer token |

### `MemberResponse`

| Property | Type | Notes |
|---|---|---|
| `id` | string | Member identifier |
| `name` | string | Member name |
| `gender` | string | Gender value supplied at registration |
| `description` | string | Member description |
| `city` | string | City |
| `country` | string | Country |
| `imageUrl` | string or null | Profile image URL, when available |
| `dateOfBirth` | date string | `YYYY-MM-DD` |
| `createdAt` | date-time string | Member creation time |
| `lastActiveAt` | date-time string | Last active time |
| `photos` | array of `PhotoResponse` | Member photo records |

### `PhotoResponse`

| Property | Type | Notes |
|---|---|---|
| `id` | string | Photo identifier |
| `url` | string | Stored photo URL |
| `publicId` | string or null | Provider-side identifier, when present |

## HTTP status codes

The status codes returned by each action are listed above. In particular, account registration and login return `200 OK` on success; this API does not currently return `201 Created` for registration. Member endpoints return `401 Unauthorized` when authentication is absent or invalid, `404 Not Found` for missing resources where noted, and upload validation failures return `400 Bad Request`.

ASP.NET Core's API controller and authorization middleware may format framework-generated error bodies differently from the plain error messages returned explicitly by account actions. Clients should rely primarily on the HTTP status code and handle response bodies as appropriate.
