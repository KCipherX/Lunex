using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Lunex.Domain.Enitities.Photos;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Domain.Enitities.Members;

public sealed class Member
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public ICollection<Photo> Photos { get; set; } = [];

    /// <summary>
    /// navigational property
    /// </summary>
    [ForeignKey(nameof(Id)), JsonIgnore]
    public User User { get; set; } = null!;
}