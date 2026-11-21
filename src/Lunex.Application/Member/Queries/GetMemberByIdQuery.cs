using Lunex.Application.Member.Results;

using MediatR;

namespace Lunex.Application.Member.Queries;

public sealed record GetMemberByIdQuery(string MemberId) : IRequest<MemberResult?>;