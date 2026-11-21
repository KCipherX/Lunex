using Lunex.Application.Member.Results;

using MediatR;

namespace Lunex.Application.Member.Queries;

public sealed record GetMemberByUserIdQuery(string UserId) : IRequest<MemberResult?>;