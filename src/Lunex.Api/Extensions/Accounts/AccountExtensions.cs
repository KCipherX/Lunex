using System.Security.Cryptography;
using System.Text;

using Lunex.Application.Common.Services.Abstractions;
using Lunex.Contracts.Accounts.Common.Responses;
using Lunex.Contracts.Accounts.Register.Requests;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Api.Extensions.Accounts;

public static class AccountExtensions
{
    public static UserResponse ToDto(this User user, IJwtTokenGenerator jwtTokenGenerator) => new(
        user.Id,
        user.Email,
        user.Name,
        string.Empty,
        jwtTokenGenerator.GenerateToken(user));

    public static User FromDto(this RegisterUserRequest request, HMACSHA512 hmac = null!)
    {
        hmac ??= new();
        var userId = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = userId,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key,
            Member = new()
            {
                Id = userId,
                Name = request.Name,
                Gender = request.Gender,
                City = request.City,
                Country = request.Country,
                DateOfBirth = request.DateOfBirth,
                Description = request.Description ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow,
            }
        };

        return user;
    }
}
