using Lunex.Domain.Identity.ValueObjects;

namespace Lunex.Domain.Identity.Services;

public interface IPasswordHasher
{
    (PasswordHash Hash, PasswordSalt Salt) HashPassword(string password);
    bool VerifyPassword(string password, PasswordHash hash, PasswordSalt salt);
}