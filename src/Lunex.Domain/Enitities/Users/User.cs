namespace Lunex.Domain.Enitities.Users;

public sealed class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string Email { get; set; }
    public static User Empty => new()
    {
        Id = string.Empty,
        Name = string.Empty,
        Email = string.Empty
    };
}