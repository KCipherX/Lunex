using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Services.Abstractions;

public interface IAccountService
{
    Task<User?> RegisterAsync(User user, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
