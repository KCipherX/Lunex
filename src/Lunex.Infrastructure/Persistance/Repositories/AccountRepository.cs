using System;
using System.Collections.Generic;
using System.Text;

using Lunex.Application.Accounts.Persistance.Abstractions;
using Lunex.Domain.Enitities.Users;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class AccountRepository(ApplicationDbContext dbContext) : IAccountRepository
{
    public async Task<User> RegisterAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
    }
}
