namespace Lunex.Contracts.Accounts.Login;

public sealed record LoginRequest(
    string Email,
    string Password);
