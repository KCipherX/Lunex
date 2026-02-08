using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Accounts.Services.Abstractions;

public interface IAccountService
{
    Task<User> RegisterAsync(User user);
}
