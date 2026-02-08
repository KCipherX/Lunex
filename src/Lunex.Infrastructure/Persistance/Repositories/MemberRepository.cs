using Lunex.Application.Members.Persistence.Abstractions;
using Lunex.Domain.Enitities.Users;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance.Repositories;

public sealed class MemberRepository(ApplicationDbContext dbContext) : IMemberRepository
{
    public async Task<IReadOnlyList<User>> GetAsync(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users.AsNoTracking().ToListAsync(
            cancellationToken: cancellationToken);
        
        return users;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            predicate: u => u.Id == id, 
            cancellationToken: cancellationToken);

        return user;
    }
}
