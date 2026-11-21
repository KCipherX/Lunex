using System.ComponentModel.DataAnnotations;

namespace Lunex.Contracts.Members.Requests;

public sealed record MemberRequest(
    string Name,
    string Email);

/// <summary>
/// Request to create a member profile for a registered user.
/// </summary>
/// <param name="Name">Display name (2-100 characters).</param>
/// <param name="Gender">Gender (max 20 characters).</param>
/// <param name="DateOfBirth">Date of birth.</param>
/// <param name="City">City (max 100 characters).</param>
/// <param name="Country">Country (max 100 characters).</param>
/// <param name="Description">Optional bio/description (max 500 characters).</param>
public sealed record MemberCreateRequest(
    [Required, MinLength(2), MaxLength(100)] string Name,
    [Required, MaxLength(20)] string Gender,
    [Required] DateOnly DateOfBirth,
    [Required, MaxLength(100)] string City,
    [Required, MaxLength(100)] string Country,
    [MaxLength(500)] string? Description = null);

public sealed record MemberUpdateRequest(
    string DisplayName,
    string Description,
    string City,
    string Country);