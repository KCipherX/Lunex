namespace Lunex.Contracts.Accounts.Login.Requests;

/// <summary>
/// Request to authenticate a user.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password.</param>
public sealed record LoginUserRequest(
    string Email,
    string Password);
