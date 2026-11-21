using Lunex.Application.Identity.Commands;
using Lunex.Domain.Identity.Repositories;
using Lunex.Domain.Identity.Services;

using MediatR;

namespace Lunex.Application.Identity.Handlers;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.ChangePassword(command.CurrentPassword, command.NewPassword, passwordHasher);

        await userRepository.UpdateAsync(user, cancellationToken);

        user.ClearDomainEvents();

        return true;
    }
}