using Lunex.Application.Common.Pagination;
using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Application.Members.Persistence.Abstractions;

public interface IMemberRepository
{
    Task<PaginatedResult<Member>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Photo>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task UpdateAsync(Member member, CancellationToken cancellationToken);
    Task<bool> CommitAsync(CancellationToken cancellationToken);
    Task AddPhotoAsync(string memberId, Photo photo, CancellationToken cancellationToken);
    Task<bool> RemovePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
    Task<bool> SetMainPhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
}
