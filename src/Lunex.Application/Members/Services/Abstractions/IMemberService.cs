using Lunex.Application.Common.Pagination;
using Lunex.Contracts.Members.Requests;
using Lunex.Contracts.Members.Responses;

namespace Lunex.Application.Members.Services.Abstractions;

public interface IMemberService
{
    Task<PaginatedResult<MemberResponse>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<MemberResponse?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PhotoResponse>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task<PhotoResponse> UploadPhotoAsync(string memberId, Stream fileStream, string fileName, CancellationToken cancellationToken);
    Task<bool> DeletePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
    Task<bool> SetMainPhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
}
