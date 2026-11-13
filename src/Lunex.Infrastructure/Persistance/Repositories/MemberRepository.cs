using Lunex.Application.Members.Persistence.Abstractions;
using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class MemberRepository(ApplicationDbContext db) : IMemberRepository
{
    public async Task<bool> CommitAsync(CancellationToken cancellationToken)
    {
        var rowsAffected = await db.SaveChangesAsync(cancellationToken);

        return rowsAffected > 0;
    }

    public async Task<IReadOnlyList<Member>> GetAsync(CancellationToken cancellationToken)
    {
        var members = await db.Members
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return members;
    }

    public async Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var member = await db.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return member;
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosAsync(string id, CancellationToken cancellationToken)
    {
        var photo = await db.Members
            .AsNoTracking()
            .Where(m => m.Id == id)
            .SelectMany(m => m.Photos)
            .ToListAsync(cancellationToken);

        return photo;
    }

    public async Task UpdateAsync(Member member, CancellationToken cancellationToken)
    {
        db.Entry(member).State = EntityState.Modified;

        await db.SaveChangesAsync(cancellationToken);
    }
}
