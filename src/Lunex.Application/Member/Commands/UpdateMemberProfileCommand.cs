using Lunex.Application.Member.Results;

using MediatR;

namespace Lunex.Application.Member.Commands;

public sealed record UpdateMemberProfileCommand(
    string MemberId,
    string? Name = null,
    string? Gender = null,
    DateOnly? DateOfBirth = null,
    string? City = null,
    string? Country = null,
    string? Description = null,
    string? ImageUrl = null) : IRequest<MemberResult?>;