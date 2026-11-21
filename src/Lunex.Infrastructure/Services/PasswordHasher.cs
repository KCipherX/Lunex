using System.Security.Cryptography;

using Lunex.Domain.Identity.Services;
using Lunex.Domain.Identity.ValueObjects;

namespace Lunex.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public (PasswordHash Hash, PasswordSalt Salt) HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(HashSize);

        return (
            PasswordHash.Create(Convert.ToBase64String(hash)),
            PasswordSalt.Create(Convert.ToBase64String(salt)));
    }

    public bool VerifyPassword(string password, PasswordHash hash, PasswordSalt salt)
    {
        var saltBytes = Convert.FromBase64String(salt.Value);
        var hashBytes = Convert.FromBase64String(hash.Value);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(HashSize);

        return CryptographicOperations.FixedTimeEquals(hashBytes, computedHash);
    }
}