using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Services.Implementations;

public sealed class AccountService(IAccountRepository accountRepository) : IAccountService
{
    public async Task<User?> RegisterAsync(User user, CancellationToken cancellationToken)
    {
        var registeredUser = await accountRepository.RegisterAsync(user, cancellationToken);
        return registeredUser;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await accountRepository.EmailExistsAsync(email, cancellationToken);
        return emailExists;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await accountRepository.GetByEmailAsync(email, cancellationToken);
        return emailExists;
    }
}
