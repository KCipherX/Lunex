using Lunex.Domain.Common;
using Lunex.Domain.Identity.Events;
using Lunex.Domain.Identity.Services;
using Lunex.Domain.Identity.ValueObjects;

namespace Lunex.Domain.Identity.Aggregates;

public sealed class User : IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public string Id { get; private set; } = string.Empty;
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public PasswordSalt PasswordSalt { get; private set; }
    public string? Name { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private User() { }

    public static User Register(Email email, string password, IPasswordHasher passwordHasher, string? name = null)
    {
        var (hash, salt) = passwordHasher.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        user._domainEvents.Add(new UserRegistered
        {
            UserId = user.Id,
            Email = email
        });

        return user;
    }

    public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
    {
        return passwordHasher.VerifyPassword(password, PasswordHash, PasswordSalt);
    }

    public void ChangePassword(string currentPassword, string newPassword, IPasswordHasher passwordHasher)
    {
        if (!VerifyPassword(currentPassword, passwordHasher))
            throw new InvalidOperationException("Current password is incorrect");

        var (hash, salt) = passwordHasher.HashPassword(newPassword);
        PasswordHash = hash;
        PasswordSalt = salt;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        _domainEvents.Add(new UserLoggedIn
        {
            UserId = Id,
            LoginTime = LastLoginAt.Value
        });
    }

    public void UpdateProfile(string? name, string? imageUrl)
    {
        Name = name;
        ImageUrl = imageUrl;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}