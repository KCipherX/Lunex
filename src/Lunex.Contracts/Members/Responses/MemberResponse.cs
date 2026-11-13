namespace Lunex.Contracts.Members.Responses;

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