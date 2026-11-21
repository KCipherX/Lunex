using System.ComponentModel.DataAnnotations;

namespace Lunex.Contracts.Accounts.Register.Requests;

/// <summary>
/// Request to register a new user account (authentication only).
/// </summary>
/// <param name="Email">User's email address (valid email format, max 256 characters).</param>
/// <param name="Password">User's password (8-100 characters).</param>
/// <param name="Name">Optional display name (2-100 characters).</param>
public sealed record RegisterUserRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password,
    [MinLength(2), MaxLength(100)] string? Name = null);
