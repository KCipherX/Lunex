using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Members.Services.Abstractions;

public interface IMemberService
{
    Task<IReadOnlyList<User>> GetAsync(CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
