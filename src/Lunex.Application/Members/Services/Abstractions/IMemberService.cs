using Lunex.Application.Common.Pagination;
using Lunex.Contracts.Members.Responses;

namespace Lunex.Application.Members.Services.Abstractions;

public interface IMemberService
{
    Task<PaginatedResult<MemberResponse>> GetAsync(MemberParams memberParams, CancellationToken cancellationToken);
    Task<MemberResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PhotoResponse>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task<PhotoResponse> UploadPhotoAsync(string memberId, Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<bool> DeletePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
    Task<bool> SetMainPhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
}
