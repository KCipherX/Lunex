using Lunex.Application.Member.Results;
using Lunex.Contracts.Members.Requests;
using Lunex.Contracts.Members.Responses;
using Lunex.Domain.Common.Pagination;
using Lunex.Domain.Member.ValueObjects;

namespace Lunex.Api.Extensions.Members;

/// <summary>
/// Extension methods for mapping between Member Contracts and Application layer types.
/// </summary>
public static class MemberMappingExtensions
{
    /// <summary>
    /// Maps a MemberCreateRequest to CreateMemberProfileCommand primitives.
    /// </summary>
    public static (string UserId, string Name, string Gender, DateOnly DateOfBirth, string City, string Country, string? Description)
        ToCommandPrimitives(this MemberCreateRequest request, string userId)
        => (userId, request.Name, request.Gender, request.DateOfBirth, request.City, request.Country, request.Description);

    /// <summary>
    /// Maps a MemberUpdateRequest to UpdateMemberProfileCommand primitives.
    /// </summary>
    public static (string MemberId, string? Name, string? Gender, DateOnly? DateOfBirth, string? City, string? Country, string? Description, string? ImageUrl)
        ToCommandPrimitives(this MemberUpdateRequest request, string memberId)
        => (memberId, request.DisplayName, null, null, request.City, request.Country, request.Description, null);

    /// <summary>
    /// Maps a MemberResult to MemberResponse.
    /// </summary>
    public static MemberResponse ToMemberResponse(this MemberResult member)
        => new(
            member.Id,
            member.Name,
            member.Gender.ToDisplayString(),
            member.Description,
            member.Location.City,
            member.Location.Country,
            member.ImageUrl,
            member.DateOfBirth.Value,
            member.CreatedAt,
            member.LastActiveAt,
            member.Photos.Select(p => p.ToPhotoResponse()).ToList());

    /// <summary>
    /// Maps a PhotoResult to PhotoResponse.
    /// </summary>
    public static PhotoResponse ToPhotoResponse(this PhotoResult photo)
        => new PhotoResponse(photo.Id, photo.Url, photo.PublicId);

    /// <summary>
    /// Maps a PaginatedResult<MemberResult> to PaginatedResult<MemberResponse>.
    /// </summary>
    public static PaginatedResult<MemberResponse> ToPaginatedResponse(this PaginatedResult<MemberResult> paginatedResult)
        => new()
        {
            Metadata = paginatedResult.Metadata,
            Items = paginatedResult.Items.Select(m => m.ToMemberResponse()).ToList()
        };
}