using MediatR;

namespace Lunex.Application.Member.Commands;

public sealed record RemovePhotoCommand(
    string MemberId,
    string PhotoId) : IRequest<bool>;