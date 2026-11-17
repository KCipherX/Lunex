using System.ComponentModel.DataAnnotations;

namespace Lunex.Contracts.Accounts.Register.Requests;

/// <summary>
/// Request to register a new user account.
/// </summary>
/// <param name="Name">User's display name (2-100 characters).</param>
/// <param name="Email">User's email address (valid email format, max 256 characters).</param>
/// <param name="Password">User's password (8-100 characters).</param>
/// <param name="Gender">User's gender (max 20 characters).</param>
/// <param name="City">User's city (max 100 characters).</param>
/// <param name="Country">User's country (max 100 characters).</param>
/// <param name="DateOfBirth">User's date of birth.</param>
/// <param name="Description">Optional user bio/description (max 500 characters).</param>
public sealed record RegisterUserRequest(
    [Required, MinLength(2), MaxLength(100)] string Name,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password,
    [Required, MaxLength(20)] string Gender,
    [Required, MaxLength(100)] string City,
    [Required, MaxLength(100)] string Country,
    [Required] DateOnly DateOfBirth,
    [MaxLength(500)] string? Description = null);
