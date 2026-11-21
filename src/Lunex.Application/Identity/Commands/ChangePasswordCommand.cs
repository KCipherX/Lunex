using MediatR;

namespace Lunex.Application.Identity.Commands;

public sealed record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<bool>;