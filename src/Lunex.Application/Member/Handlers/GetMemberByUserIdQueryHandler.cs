using Lunex.Application.Member.Queries;
using Lunex.Application.Member.Results;
using Lunex.Domain.Member.Repositories;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class GetMemberByUserIdQueryHandler(
    IMemberRepository memberRepository) : IRequestHandler<GetMemberByUserIdQuery, MemberResult?>
{
    public async Task<MemberResult?> Handle(GetMemberByUserIdQuery query, CancellationToken cancellationToken)
    {
        var member = await memberRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return member is not null ? MemberResult.FromDomain(member) : null;
    }
}