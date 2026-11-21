namespace Lunex.Domain.Identity.ValueObjects;

public readonly record struct PasswordHash
{
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password hash cannot be empty", nameof(value));

        return new PasswordHash(value);
    }

    public static implicit operator string(PasswordHash hash) => hash.Value;
    public static explicit operator PasswordHash(string value) => Create(value);
}