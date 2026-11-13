using Lunex.Application.Common.Pagination;
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

    public async Task<PaginatedResult<Member>> GetAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = db.Members.AsNoTracking().OrderBy(m => m.Name);
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

    public async Task AddPhotoAsync(string memberId, Photo photo, CancellationToken cancellationToken)
    {
        var member = await db.Members.FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);
        if (member is null)
        {
            throw new InvalidOperationException($"Member with id {memberId} not found");
        }

        member.Photos.Add(photo);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemovePhotoAsync(string memberId, string photoId, CancellationToken cancellationToken)
    {
        var member = await db.Members
            .Include(m => m.Photos)
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

        if (member is null)
        {
            return false;
        }

        var photo = member.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo is null)
        {
            return false;
        }

        member.Photos.Remove(photo);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetMainPhotoAsync(string memberId, string photoId, CancellationToken cancellationToken)
    {
        var member = await db.Members
            .Include(m => m.Photos)
            .FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken);

        if (member is null)
        {
            return false;
        }

        var photo = member.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo is null)
        {
            return false;
        }

        member.ImageUrl = photo.Url;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
