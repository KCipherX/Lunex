using Lunex.Domain.Enitities.Users;

namespace Lunex.Application.Common.Services.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}