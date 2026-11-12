using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;
using Lunex.Domain.Enitities.Users;
using Lunex.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Lunex.Infrastructure.Persistance;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Photo> Photos => Set<Photo>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.Schemas.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}