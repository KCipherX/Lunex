using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Persistance.Abstractions;

public interface IAccountRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task<User?> RegisterAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
