using Lunex.Application.Member.Results;
using Lunex.Domain.Common.Pagination;

namespace Lunex.Application.Member.Services;

public interface IMemberService
{
    Task<PaginatedResult<MemberResult>> GetAsync(MemberParams memberParams, CancellationToken cancellationToken);
    Task<MemberResult?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MemberResult?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<MemberResult> CreateProfileAsync(string userId, string name, string gender, DateOnly dateOfBirth, string city, string country, string? description, CancellationToken cancellationToken);
    Task<MemberResult?> UpdateProfileAsync(string memberId, string? name, string? gender, DateOnly? dateOfBirth, string? city, string? country, string? description, string? imageUrl, CancellationToken cancellationToken);
    Task<IReadOnlyList<PhotoResult>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task<PhotoResult> AddPhotoAsync(string memberId, Stream fileStream, string fileName, bool isMain, CancellationToken cancellationToken);
    Task<bool> RemovePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
    Task<bool> SetMainPhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
    Task UpdateLastActiveAsync(string memberId, CancellationToken cancellationToken);
}