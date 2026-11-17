namespace Lunex.Contracts.Members.Responses;

/// <summary>
/// Represents a member profile response.
/// </summary>
/// <param name="Id">Unique identifier of the member.</param>
/// <param name="Name">Display name of the member.</param>
/// <param name="Gender">Gender of the member.</param>
/// <param name="Description">Member's bio/description.</param>
/// <param name="City">Member's city.</param>
/// <param name="Country">Member's country.</param>
/// <param name="ImageUrl">URL to the member's profile image.</param>
/// <param name="DateOfBirth">Member's date of birth.</param>
/// <param name="CreatedAt">When the member profile was created.</param>
/// <param name="LastActiveAt">When the member was last active.</param>
/// <param name="Photos">List of member's photos.</param>
public sealed record MemberResponse(
    string Id,
    string Name,
    string Gender,
    string Description,
    string City,
    string Country,
    string? ImageUrl,
    DateOnly DateOfBirth,
    DateTime CreatedAt,
    DateTime LastActiveAt,
    IReadOnlyList<PhotoResponse> Photos);