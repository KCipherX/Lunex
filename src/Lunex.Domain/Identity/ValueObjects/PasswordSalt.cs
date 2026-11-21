namespace Lunex.Domain.Identity.ValueObjects;

public readonly record struct PasswordSalt
{
    public string Value { get; }

    private PasswordSalt(string value) => Value = value;

    public static PasswordSalt Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password salt cannot be empty", nameof(value));

        return new PasswordSalt(value);
    }

    public static implicit operator string(PasswordSalt salt) => salt.Value;
    public static explicit operator PasswordSalt(string value) => Create(value);
}