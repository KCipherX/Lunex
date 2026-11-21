namespace Lunex.Domain.Member.ValueObjects;

public readonly record struct DateOfBirth
{
    public DateOnly Value { get; }

    private DateOfBirth(DateOnly value) => Value = value;

    public static DateOfBirth Create(DateOnly value)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - value.Year;
        if (value > today.AddYears(-age)) age--;

        if (age < 13)
            throw new ArgumentException("Member must be at least 13 years old", nameof(value));
        if (age > 120)
            throw new ArgumentException("Invalid date of birth", nameof(value));

        return new DateOfBirth(value);
    }

    public int CalculateAge() => CalculateAge(DateOnly.FromDateTime(DateTime.UtcNow));

    public int CalculateAge(DateOnly asOfDate)
    {
        var age = asOfDate.Year - Value.Year;
        if (Value > asOfDate.AddYears(-age)) age--;
        return age;
    }

    public static implicit operator DateOnly(DateOfBirth dob) => dob.Value;
    public static explicit operator DateOfBirth(DateOnly value) => Create(value);
    public override string ToString() => Value.ToString("yyyy-MM-dd");
}