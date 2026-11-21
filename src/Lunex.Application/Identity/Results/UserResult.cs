namespace Lunex.Application.Identity.Results;

public sealed record UserResult(
    string Id,
    string Email,
    string? Name,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? LastLoginAt)
{
    public static UserResult FromDomain(Lunex.Domain.Identity.Aggregates.User user)
    {
        return new UserResult(
            user.Id,
            user.Email.Value,
            user.Name,
            user.ImageUrl,
            user.CreatedAt,
            user.LastLoginAt);
    }
}