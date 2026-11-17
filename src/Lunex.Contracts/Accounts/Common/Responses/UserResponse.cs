namespace Lunex.Contracts.Accounts.Common.Responses;

/// <summary>
/// Represents an authenticated user response with JWT token.
/// </summary>
/// <param name="Id">Unique identifier of the user.</param>
/// <param name="Email">User's email address.</param>
/// <param name="Name">User's display name.</param>
/// <param name="ImageUrl">URL to the user's profile image.</param>
/// <param name="Token">JWT access token for authentication.</param>
public sealed record UserResponse(
    string Id,
    string Email,
    string Name,
    string ImageUrl,
    string Token);
