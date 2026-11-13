namespace Lunex.Contracts.Members.Responses;

public sealed record PhotoUploadResult(
    string Url,
    string PublicId,
    int Width,
    int Height,
    string Format);