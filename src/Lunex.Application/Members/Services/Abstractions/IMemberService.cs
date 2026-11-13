using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Members.Services.Abstractions;

public interface IMemberService
{
    Task<IReadOnlyList<Member>> GetAsync(CancellationToken cancellationToken);
    Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Photo>> GetPhotosAsync(string id, CancellationToken cancellationToken);
    Task UpdateAsync(Member member, CancellationToken cancellationToken);
}
