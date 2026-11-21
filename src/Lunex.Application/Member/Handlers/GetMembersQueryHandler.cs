using Lunex.Application.Member.Queries;
using Lunex.Application.Member.Results;
using Lunex.Domain.Common.Pagination;
using Lunex.Domain.Member.Repositories;

using MediatR;

namespace Lunex.Application.Member.Handlers;

public sealed class GetMembersQueryHandler(
    IMemberRepository memberRepository) : IRequestHandler<GetMembersQuery, PaginatedResult<MemberResult>>
{
    public async Task<PaginatedResult<MemberResult>> Handle(GetMembersQuery query, CancellationToken cancellationToken)
    {
        var paginatedMembers = await memberRepository.GetAsync(query.Params, cancellationToken);

        return new PaginatedResult<MemberResult>
        {
            Metadata = paginatedMembers.Metadata,
            Items = paginatedMembers.Items.Select(MemberResult.FromDomain).ToList()
        };
    }
}