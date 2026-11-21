using Lunex.Application.Member.Queries;
using Lunex.Application.Member.Results;
using Lunex.Domain.Member.Repositories;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class GetMemberByIdQueryHandler(
    IMemberRepository memberRepository) : IRequestHandler<GetMemberByIdQuery, MemberResult?>
{
    public async Task<MemberResult?> Handle(GetMemberByIdQuery query, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByIdAsync(query.MemberId, cancellationToken);
        return member is not null ? MemberResult.FromDomain(member) : null;
    }
}