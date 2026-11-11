namespace Lunex.Contracts.Accounts.Common.Responses;

public sealed record UserResponse(
    string Id,
    string Email,
    string Name,
    string ImageUrl,
    string Token);
