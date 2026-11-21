namespace Lunex.Application.Member.Results;

public sealed record PhotoResult(
    string Id,
    string Url,
    string? PublicId,
    bool IsMain,
    DateTime CreatedAt)
{
    public static PhotoResult FromDomain(Lunex.Domain.Member.Aggregates.Photo photo)
    {
        return new PhotoResult(
            photo.Id,
            photo.Url,
            photo.PublicId,
            photo.IsMain,
            photo.CreatedAt);
    }
}