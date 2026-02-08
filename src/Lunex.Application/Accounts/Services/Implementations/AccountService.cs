using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Services.Implementations;

public sealed class AccountService(IAccountRepository accountRepository) : IAccountService
{
    public async Task<User> RegisterAsync(User user)
    {
        var registeredUser = await accountRepository.RegisterAsync(user);
        return registeredUser;
    }
}
