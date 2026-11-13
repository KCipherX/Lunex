using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Domain.Enitities.Users;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class AccountRepository(ApplicationDbContext db) : IAccountRepository
{
    public async Task<User?> RegisterAsync(User user, CancellationToken cancellationToken)
    {
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await db.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email.ToLower() == email.ToLower(), cancellationToken);

        return emailExists;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);

        return user;
    }
}
