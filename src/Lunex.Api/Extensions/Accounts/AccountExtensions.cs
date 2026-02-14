using System.Security.Cryptography;
using System.Text;

using Lunex.Application.Common.Services.Abstractions;
using Lunex.Contracts.Members.Requests;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Api.Extensions.Accounts;

public static class AccountExtensions
{
    public static UserResponse ToDto(this User user, IJwtTokenGenerator jwtTokenGenerator) =>
        new(user.Id,
            user.Email,
            user.Name,
            string.Empty,
            jwtTokenGenerator.GenerateToken(user));

    public static User ToDto(this RegisterRequest request, HMACSHA512 hmac) =>
         new()
         {
             Name = request.Name,
             Email = request.Email,
             PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
             PasswordSalt = hmac.Key
         };
}
