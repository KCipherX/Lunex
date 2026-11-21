using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Identity.Commands;
using Lunex.Application.Identity.Results;
using Lunex.Domain.Identity.Repositories;
using Lunex.Domain.Identity.Services;
using Lunex.Domain.Identity.ValueObjects;

using MediatR;

namespace Lunex.Application.Identity.Handlers;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginUserCommand, TokenResult>
{
    public async Task<TokenResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.VerifyPassword(command.Password, passwordHasher))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        user.RecordLogin();

        await userRepository.UpdateAsync(user, cancellationToken);

        var token = jwtTokenGenerator.GenerateToken(user);

        user.ClearDomainEvents();

        return new TokenResult(token);
    }
}