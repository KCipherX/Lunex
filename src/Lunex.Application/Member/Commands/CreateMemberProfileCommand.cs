using Lunex.Application.Member.Results;

using MediatR;

namespace Lunex.Application.Member.Commands;

public sealed record CreateMemberProfileCommand(
    string UserId,
    string Name,
    string Gender,
    DateOnly DateOfBirth,
    string City,
    string Country,
    string? Description = null) : IRequest<MemberResult>;