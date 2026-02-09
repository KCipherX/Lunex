using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Services.Implementations;

public sealed class AccountService(IAccountRepository accountRepository) : IAccountService
{
    public async Task<User?> RegisterAsync(User user)
    {
        var registeredUser = await accountRepository.RegisterAsync(user);
        return registeredUser;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var emailExists = await accountRepository.EmailExistsAsync(email);
        return emailExists;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var emailExists = await accountRepository.GetByEmailAsync(email);
        return emailExists;
    }
}
