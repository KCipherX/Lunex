using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

namespace Lunex.Domain.Enitities.Users;

public sealed class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? ImageUrl { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

    /// <summary>
    /// navigational property
    /// </summary>
    public Member Member { get; set; } = null!;
    public ICollection<Photo> Photos { get; set; } = [];
}
