using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Persistance.Abstractions;

public interface IAccountRepository
{
    Task<User> RegisterAsync(User user);
}
