using System.Text.Json.Serialization;

using Lunex.Domain.Enitities.Members;

namespace Lunex.Domain.Enitities.Photos;

public sealed class Photo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Url { get; set; }
    public string? PublicId { get; set; }

    /// <summary>
    /// navigational property
    /// </summary>
    /// 
    [JsonIgnore]
    public Member Member { get; set; } = null!;
}