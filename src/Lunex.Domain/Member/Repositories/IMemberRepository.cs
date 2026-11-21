using Lunex.Domain.Common.Pagination;

using MemberAggregate = Lunex.Domain.Member.Aggregates.Member;

namespace Lunex.Domain.Member.Repositories;

public interface IMemberRepository
{
    Task<MemberAggregate?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default);
    Task<MemberAggregate?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(MemberAggregate member, CancellationToken cancellationToken = default);
    Task UpdateAsync(MemberAggregate member, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<MemberAggregate>> GetAsync(MemberParams memberParams, CancellationToken cancellationToken = default);
}