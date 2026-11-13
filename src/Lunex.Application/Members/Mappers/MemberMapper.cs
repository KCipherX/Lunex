using Lunex.Contracts.Members.Responses;
using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Application.Members.Mappers;

public static class MemberMapper
{
    public static MemberResponse ToResponse(this Member member)
    {
        return new MemberResponse(
            member.Id,
            member.Name,
            member.Gender,
            member.Description,
            member.City,
            member.Country,
            member.ImageUrl,
            member.DateOfBirth,
            member.CreatedAt,
            member.LastActiveAt,
            member.Photos?.Select(p => p.ToResponse()).ToList() ?? []);
    }

    public static PhotoResponse ToResponse(this Photo photo)
    {
        return new PhotoResponse(
            photo.Id,
            photo.Url,
            photo.PublicId);
    }

    public static IReadOnlyList<MemberResponse> ToResponse(this IEnumerable<Member> members)
    {
        return [.. members.Select(m => m.ToResponse())];
    }

    public static IReadOnlyList<PhotoResponse> ToResponse(this IEnumerable<Photo> photos)
    {
        return [.. photos.Select(p => p.ToResponse())];
    }
}