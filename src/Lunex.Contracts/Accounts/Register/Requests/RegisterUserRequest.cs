using System.ComponentModel.DataAnnotations;

namespace Lunex.Contracts.Accounts.Register.Requests;

public sealed record RegisterUserRequest(
    [Required, MinLength(2), MaxLength(100)] string Name,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password,
    [Required, MaxLength(20)] string Gender,
    [Required, MaxLength(100)] string City,
    [Required, MaxLength(100)] string Country,
    [Required] DateOnly DateOfBirth,
    [MaxLength(500)] string? Description = null);
