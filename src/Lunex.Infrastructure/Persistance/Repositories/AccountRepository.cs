using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Domain.Enitities.Users;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class AccountRepository(ApplicationDbContext dbContext) : IAccountRepository
{
    public async Task<User?> RegisterAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var emailExists = await dbContext.Users
            .AnyAsync(user => user.Email.ToLower() == email.ToLower());

        return emailExists;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == email);
        return user;
    }
}
