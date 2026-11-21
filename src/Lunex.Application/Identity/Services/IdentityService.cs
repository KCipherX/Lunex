using Lunex.Application.Identity.Commands;
using Lunex.Application.Identity.Queries;
using Lunex.Application.Identity.Results;

using MediatR;

namespace Lunex.Application.Identity.Services;

public sealed class IdentityService(IMediator mediator) : IIdentityService
{
    public async Task<TokenResult> RegisterAsync(string email, string password, string? name, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(email, password, name);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<TokenResult> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(email, password);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(userId, currentPassword, newPassword);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<UserResult?> GetByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<UserResult?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var query = new GetUserByEmailQuery(email);
        return await mediator.Send(query, cancellationToken);
    }
}