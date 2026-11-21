using MediatR;

namespace Lunex.Application.Member.Commands;

public sealed record SetMainPhotoCommand(
    string MemberId,
    string PhotoId) : IRequest<bool>;