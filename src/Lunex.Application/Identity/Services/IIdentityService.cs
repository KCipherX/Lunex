using Lunex.Application.Identity.Results;

namespace Lunex.Application.Identity.Services;

public interface IIdentityService
{
    Task<TokenResult> RegisterAsync(string email, string password, string? name, CancellationToken cancellationToken);
    Task<TokenResult> LoginAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
    Task<UserResult?> GetByIdAsync(string userId, CancellationToken cancellationToken);
    Task<UserResult?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}