using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Application.Members.Persistence.Abstractions;

public interface IMemberRepository
{
    Task<IReadOnlyList<Member>> GetAsync(CancellationToken cancellationToken);
    Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Photo>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task UpdateAsync(Member member, CancellationToken cancellationToken);
    Task<bool> CommitAsync(CancellationToken cancellationToken);
    Task AddPhotoAsync(string memberId, Photo photo, CancellationToken cancellationToken);
    Task<bool> RemovePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken);
}
