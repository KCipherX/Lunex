using Lunex.Application.Identity.Results;

using MediatR;

namespace Lunex.Application.Identity.Commands;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string? Name = null) : IRequest<TokenResult>;