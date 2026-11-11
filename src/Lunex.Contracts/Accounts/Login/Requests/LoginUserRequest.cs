namespace Lunex.Contracts.Accounts.Login.Requests;

public sealed record LoginUserRequest(
    string Email,
    string Password);
