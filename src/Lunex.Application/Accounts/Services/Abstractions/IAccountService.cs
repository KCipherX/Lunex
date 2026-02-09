using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Services.Abstractions;

public interface IAccountService
{
    Task<User?> RegisterAsync(User user);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> GetByEmailAsync(string email);
}
