using Lunex.Application.Member.Results;

using MediatR;

namespace Lunex.Application.Member.Commands;

public sealed record AddPhotoCommand(
    string MemberId,
    Stream FileStream,
    string FileName,
    bool IsMain = false) : IRequest<PhotoResult>;