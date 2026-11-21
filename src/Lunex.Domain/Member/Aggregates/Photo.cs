namespace Lunex.Domain.Member.Aggregates;

public sealed class Photo
{
    public string Id { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string? PublicId { get; private set; }
    public bool IsMain { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Photo() { }

    internal static Photo Create(string url, string? publicId = null, bool isMain = false)
    {
        return new Photo
        {
            Id = Guid.NewGuid().ToString(),
            Url = url,
            PublicId = publicId,
            IsMain = isMain,
            CreatedAt = DateTime.UtcNow
        };
    }

    internal void SetAsMain() => IsMain = true;
    internal void UnsetAsMain() => IsMain = false;
}