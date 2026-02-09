using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Lunex.Application.Common.Configurations;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Domain.Enitities.Users;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lunex.Application.Common.Services.Implementations;

public sealed class JwtTokenGenerator(IOptions<JwtSettings> _jwtOptions, IDateTimeProvider _dateTimeProvider) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = _jwtOptions.Value;

    public string GenerateToken(User user)
    {
        if (Guid.Parse(user.Id) == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(user.Id));

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.GivenName, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.NameId, user.Id)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}