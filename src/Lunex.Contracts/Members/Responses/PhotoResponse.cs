namespace Lunex.Contracts.Members.Responses;

public sealed record PhotoResponse(
    string Id,
    string Url,
    string? PublicId);