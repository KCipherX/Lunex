namespace Lunex.Domain.Member.ValueObjects;

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    PreferNotToSay = 4
}

public static class GenderExtensions
{
    public static Gender Parse(string value)
    {
        return value?.ToLowerInvariant() switch
        {
            "male" => Gender.Male,
            "female" => Gender.Female,
            "other" => Gender.Other,
            "prefernotosay" or "prefer_not_to_say" => Gender.PreferNotToSay,
            _ => throw new ArgumentException($"Invalid gender value: {value}", nameof(value))
        };
    }

    public static bool TryParse(string value, out Gender gender)
    {
        try
        {
            gender = Parse(value);
            return true;
        }
        catch
        {
            gender = default;
            return false;
        }
    }

    public static string ToDisplayString(this Gender gender) => gender switch
    {
        Gender.Male => "Male",
        Gender.Female => "Female",
        Gender.Other => "Other",
        Gender.PreferNotToSay => "Prefer not to say",
        _ => throw new ArgumentOutOfRangeException(nameof(gender))
    };
}