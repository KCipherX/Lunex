using Lunex.Application.Common.Services.Abstractions;
using Lunex.Domain.Identity.Aggregates;
using Lunex.Domain.Identity.Services;
using Lunex.Domain.Identity.ValueObjects;
using Lunex.Domain.Member.Aggregates;
using Lunex.Domain.Member.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lunex.Infrastructure.Persistance.Seeder;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        logger.LogInformation("Starting database seeding...");

        await SeedUsersAsync(dbContext, logger, passwordHasher);
        await SeedMembersAsync(dbContext, logger, dateTimeProvider);
        await SeedPhotosAsync(dbContext, logger);

        logger.LogInformation("Database seeding completed.");
    }

    private static async Task SeedUsersAsync(ApplicationDbContext dbContext, ILogger logger, IPasswordHasher passwordHasher)
    {
        if (await dbContext.Users.AnyAsync())
        {
            logger.LogInformation("Users already exist, skipping user seeding.");
            return;
        }

        var users = new List<User>
        {
            CreateUser("admin@lunex.com", "Admin User", "Admin@123", passwordHasher),
            CreateUser("john.doe@lunex.com", "John Doe", "Password@123", passwordHasher),
            CreateUser("jane.smith@lunex.com", "Jane Smith", "Password@123", passwordHasher),
            CreateUser("bob.wilson@lunex.com", "Bob Wilson", "Password@123", passwordHasher),
            CreateUser("alice.brown@lunex.com", "Alice Brown", "Password@123", passwordHasher),
        };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} users.", users.Count);
    }

    private static async Task SeedMembersAsync(ApplicationDbContext dbContext, ILogger logger, IDateTimeProvider dateTimeProvider)
    {
        if (await dbContext.Members.AnyAsync())
        {
            logger.LogInformation("Members already exist, skipping member seeding.");
            return;
        }

        var users = await dbContext.Users.ToListAsync();
        if (users.Count <= 0)
        {
            logger.LogWarning("No users found, cannot seed members.");
            return;
        }

        var now = dateTimeProvider.UtcNow;
        var members = new List<Member>
        {
            CreateMember(users[0].Id, "Admin User", "Male", "System administrator", "New York", "USA", new DateOnly(1990, 1, 15), now),
            CreateMember(users[1].Id, "John Doe", "Male", "Software developer passionate about clean code", "San Francisco", "USA", new DateOnly(1992, 5, 20), now),
            CreateMember(users[2].Id, "Jane Smith", "Female", "UX designer who loves creating beautiful interfaces", "London", "UK", new DateOnly(1993, 8, 10), now),
            CreateMember(users[3].Id, "Bob Wilson", "Male", "Full-stack developer with 5+ years experience", "Toronto", "Canada", new DateOnly(1991, 11, 3), now),
            CreateMember(users[4].Id, "Alice Brown", "Female", "Data scientist and ML enthusiast", "Sydney", "Australia", new DateOnly(1994, 3, 25), now),
        };

        await dbContext.Members.AddRangeAsync(members);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} members.", members.Count);
    }

    private static async Task SeedPhotosAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        if (await dbContext.Photos.AnyAsync())
        {
            logger.LogInformation("Photos already exist, skipping photo seeding.");
            return;
        }

        var members = await dbContext.Members.ToListAsync();
        if (members.Count <= 0)
        {
            logger.LogWarning("No members found, cannot seed photos.");
            return;
        }

        // Add photos using the Member aggregate's AddPhoto method
        members[0].AddPhoto("https://api.dicebear.com/7.x/avataaars/svg?seed=admin", "admin_avatar", true);
        members[1].AddPhoto("https://api.dicebear.com/7.x/avataaars/svg?seed=john", "john_avatar", true);
        members[2].AddPhoto("https://api.dicebear.com/7.x/avataaars/svg?seed=jane", "jane_avatar", true);
        members[3].AddPhoto("https://api.dicebear.com/7.x/avataaars/svg?seed=bob", "bob_avatar", true);
        members[4].AddPhoto("https://api.dicebear.com/7.x/avataaars/svg?seed=alice", "alice_avatar", true);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded photos for {Count} members.", members.Count);
    }

    private static User CreateUser(string email, string name, string password, IPasswordHasher passwordHasher)
    {
        return User.Register(Email.Create(email), password, passwordHasher, name);
    }

    private static Member CreateMember(string userId, string name, string gender, string description, string city, string country, DateOnly dateOfBirth, DateTime now)
    {
        return Member.CreateProfile(
            userId,
            name,
            GenderExtensions.Parse(gender),
            DateOfBirth.Create(dateOfBirth),
            Location.Create(city, country),
            description);
    }
}