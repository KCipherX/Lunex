namespace Lunex.Application.Common.Services.Models;

public sealed record PhotoUploadResult(
    string Url,
    string PublicId,
    int Width,
    int Height,
    string Format);
