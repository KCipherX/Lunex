using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Domain.Enitities.Users;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class AccountRepository(ApplicationDbContext dbContext) : IAccountRepository
{
    public async Task<User?> RegisterAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await dbContext.Users
            .AnyAsync(user => user.Email.ToLower() == email.ToLower(), cancellationToken);

        return emailExists;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
        return user;
    }
}
