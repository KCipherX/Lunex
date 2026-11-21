namespace Lunex.Application.Identity.Results;

public sealed record TokenResult(
    string AccessToken,
    string TokenType = "Bearer",
    int ExpiresIn = 3600);