using Lunex.Domain.Identity.Aggregates;

namespace Lunex.Application.Common.Services.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}