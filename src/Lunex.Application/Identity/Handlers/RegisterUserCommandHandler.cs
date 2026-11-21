using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Identity.Commands;
using Lunex.Application.Identity.Results;
using Lunex.Domain.Identity.Aggregates;
using Lunex.Domain.Identity.Repositories;
using Lunex.Domain.Identity.Services;
using Lunex.Domain.Identity.ValueObjects;

using MediatR;

namespace Lunex.Application.Identity.Handlers;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RegisterUserCommand, TokenResult>
{
    public async Task<TokenResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var exists = await userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = User.Register(email, command.Password, passwordHasher, command.Name);

        await userRepository.AddAsync(user, cancellationToken);

        var token = jwtTokenGenerator.GenerateToken(user);

        user.ClearDomainEvents();

        return new TokenResult(token);
    }
}