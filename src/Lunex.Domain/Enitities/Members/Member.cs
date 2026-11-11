using Lunex.Domain.Enitities.Users;

namespace Lunex.Domain.Enitities.Members;

public sealed class Member
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string Name { get; set; }
    public required string Gender { get; set; }
    public required string Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// navigational property
    /// </summary>
    public User User { get; set; } = null!;
}