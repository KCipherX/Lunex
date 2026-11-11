namespace Lunex.Contracts.Accounts.Register.Requests;

public sealed record RegisterUserRequest(
    string Name,
    string Email,
    string Password);
