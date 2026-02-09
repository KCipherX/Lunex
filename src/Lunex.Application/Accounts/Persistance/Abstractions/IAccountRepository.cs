using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Persistance.Abstractions;

public interface IAccountRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<User?> RegisterAsync(User user);
    Task<User?> GetByEmailAsync(string email);
}
