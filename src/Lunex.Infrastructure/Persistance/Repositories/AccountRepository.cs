using Lunex.Domain.Identity.Aggregates;
using Lunex.Domain.Identity.Repositories;
using Lunex.Domain.Identity.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email.Value == email.Value, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        db.Entry(user).State = EntityState.Modified;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await db.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email.Value == email.Value, cancellationToken);
    }
}
