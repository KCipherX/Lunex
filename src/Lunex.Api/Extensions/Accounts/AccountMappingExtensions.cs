using Lunex.Application.Identity.Results;
using Lunex.Contracts.Accounts.Common.Responses;
using Lunex.Contracts.Accounts.Login.Requests;
using Lunex.Contracts.Accounts.Register.Requests;

namespace Lunex.Api.Extensions.Accounts;

/// <summary>
/// Extension methods for mapping between Account Contracts and Application layer types.
/// </summary>
public static class AccountMappingExtensions
{
    /// <summary>
    /// Maps a RegisterUserRequest to RegisterUserCommand primitives.
    /// </summary>
    public static (string Email, string Password, string? Name) ToCommandPrimitives(this RegisterUserRequest request)
        => (request.Email, request.Password, request.Name);

    /// <summary>
    /// Maps a LoginUserRequest to LoginUserCommand primitives.
    /// </summary>
    public static (string Email, string Password) ToCommandPrimitives(this LoginUserRequest request)
        => (request.Email, request.Password);

    /// <summary>
    /// Maps a TokenResult to UserResponse.
    /// </summary>
    public static UserResponse ToUserResponse(this TokenResult tokenResult, string userId, string email, string? name, string? imageUrl)
        => new UserResponse(userId, email, name ?? string.Empty, imageUrl ?? string.Empty, tokenResult.AccessToken);
}