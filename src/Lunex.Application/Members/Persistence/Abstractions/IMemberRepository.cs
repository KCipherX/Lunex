using Lunex.Domain.Enitities.Users;
namespace Lunex.Application.Members.Persistence.Abstractions;

public interface IMemberRepository
{
    Task<IReadOnlyList<User>> GetAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
