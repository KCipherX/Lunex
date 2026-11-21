using Lunex.Domain.Member.ValueObjects;

namespace Lunex.Application.Member.Results;

public sealed record MemberResult(
    string Id,
    string UserId,
    string Name,
    Gender Gender,
    DateOfBirth DateOfBirth,
    Location Location,
    string Description,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime LastActiveAt,
    IReadOnlyList<PhotoResult> Photos)
{
    public static MemberResult FromDomain(Lunex.Domain.Member.Aggregates.Member member)
    {
        return new MemberResult(
            member.Id,
            member.UserId,
            member.Name,
            member.Gender,
            member.DateOfBirth,
            member.Location,
            member.Description,
            member.ImageUrl,
            member.CreatedAt,
            member.LastActiveAt,
            member.Photos.Select(PhotoResult.FromDomain).ToList());
    }
}