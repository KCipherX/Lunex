using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Services.Interfaces;

public interface IMemberService
{
    Task<IReadOnlyList<User>> GetAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
