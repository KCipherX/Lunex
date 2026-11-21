using System.Text.RegularExpressions;

namespace Lunex.Domain.Identity.ValueObjects;

public readonly record struct Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        var trimmed = value.Trim();
        if (!EmailRegex.IsMatch(trimmed))
            throw new ArgumentException("Invalid email format", nameof(value));

        return new Email(trimmed.ToLowerInvariant());
    }

    public static implicit operator string(Email email) => email.Value;
    public static explicit operator Email(string value) => Create(value);
    public override string ToString() => Value;
}