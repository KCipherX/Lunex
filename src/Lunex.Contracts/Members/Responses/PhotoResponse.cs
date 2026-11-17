namespace Lunex.Contracts.Members.Responses;

/// <summary>
/// Represents a photo response.
/// </summary>
/// <param name="Id">Unique identifier of the photo.</param>
/// <param name="Url">URL to the photo.</param>
/// <param name="PublicId">Cloudinary public ID for the photo (used for deletion).</param>
public sealed record PhotoResponse(
    string Id,
    string Url,
    string? PublicId);