namespace Lunex.Contracts.Members.Requests;

public sealed record MemberRequest(
    string Name,
    string Email);

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Password);

public sealed record UserResponse(
    string Id,
    string Email,
    string Name,
    string ImageUrl,
    string Token);
