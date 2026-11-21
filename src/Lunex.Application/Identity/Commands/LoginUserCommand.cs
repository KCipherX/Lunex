using Lunex.Application.Identity.Results;

using MediatR;

namespace Lunex.Application.Identity.Commands;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<TokenResult>;