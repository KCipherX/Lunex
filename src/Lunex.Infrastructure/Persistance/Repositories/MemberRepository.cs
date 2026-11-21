using Lunex.Domain.Common.Pagination;
using Lunex.Domain.Member.Aggregates;
using Lunex.Domain.Member.Repositories;
using Lunex.Domain.Member.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class MemberRepository(ApplicationDbContext db) : IMemberRepository
{
    public async Task<Member?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default)
    {
        return await db.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);
    }

    public async Task<Member?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await db.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await db.Members.AddAsync(member, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Member member, CancellationToken cancellationToken = default)
    {
        db.Entry(member).State = EntityState.Modified;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await db.Members
            .AsNoTracking()
            .AnyAsync(m => m.UserId == userId, cancellationToken);
    }

    public async Task<PaginatedResult<Member>> GetAsync(MemberParams memberParams, CancellationToken cancellationToken = default)
    {
        var pageNumber = memberParams.PageNumber;
        var pageSize = memberParams.PageSize;

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = db.Members.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(memberParams.Gender))
        {
            if (GenderExtensions.TryParse(memberParams.Gender, out var gender))
            {
                query = query.Where(m => m.Gender == gender);
            }
        }

        if (!string.IsNullOrWhiteSpace(memberParams.CurrentMemberId))
        {
            query = query.Where(m => m.Id != memberParams.CurrentMemberId);
        }

        query = query.OrderBy(m => m.Name);
        var count = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Member>
        {
            Metadata = new PaginationMetaData
            {
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                PageSize = pageSize,
                TotalCount = count
            },
            Items = items
        };
    }
}
