namespace Lunex.Domain.Member.ValueObjects;

public readonly record struct Location
{
    public string City { get; }
    public string Country { get; }

    private Location(string city, string country)
    {
        City = city;
        Country = country;
    }

    public static Location Create(string city, string country)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        return new Location(city.Trim(), country.Trim());
    }

    public override string ToString() => $"{City}, {Country}";
}