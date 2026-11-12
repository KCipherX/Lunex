using System.Security.Cryptography;
using System.Text;

using Lunex.Domain.Enitities.Members;
using Lunex.Domain.Enitities.Photos;
using Lunex.Domain.Enitities.Users;

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

        logger.LogInformation("Starting database seeding...");

        await SeedUsersAsync(dbContext, logger);
        await SeedMembersAsync(dbContext, logger);
        await SeedPhotosAsync(dbContext, logger);

        logger.LogInformation("Database seeding completed.");
    }

    private static async Task SeedUsersAsync(ApplicationDbContext dbContext, ILogger logger)
    {
        if (await dbContext.Users.AnyAsync())
        {
            logger.LogInformation("Users already exist, skipping user seeding.");
            return;
        }

        var users = new List<User>
        {
            CreateUser("admin@lunex.com", "Admin User", "Admin@123"),
            CreateUser("john.doe@lunex.com", "John Doe", "Password@123"),
            CreateUser("jane.smith@lunex.com", "Jane Smith", "Password@123"),
            CreateUser("bob.wilson@lunex.com", "Bob Wilson", "Password@123"),
            CreateUser("alice.brown@lunex.com", "Alice Brown", "Password@123"),
        };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} users.", users.Count);
    }

    private static async Task SeedMembersAsync(ApplicationDbContext dbContext, ILogger logger)
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

        var members = new List<Member>
        {
            CreateMember(users[0].Id, "Admin User", "Male", "System administrator", "New York", "USA", new DateOnly(1990, 1, 15)),
            CreateMember(users[1].Id, "John Doe", "Male", "Software developer passionate about clean code", "San Francisco", "USA", new DateOnly(1992, 5, 20)),
            CreateMember(users[2].Id, "Jane Smith", "Female", "UX designer who loves creating beautiful interfaces", "London", "UK", new DateOnly(1993, 8, 10)),
            CreateMember(users[3].Id, "Bob Wilson", "Male", "Full-stack developer with 5+ years experience", "Toronto", "Canada", new DateOnly(1991, 11, 3)),
            CreateMember(users[4].Id, "Alice Brown", "Female", "Data scientist and ML enthusiast", "Sydney", "Australia", new DateOnly(1994, 3, 25)),
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

        var photos = new List<Photo>
        {
            new() { Id = Guid.NewGuid().ToString(), Url = "https://api.dicebear.com/7.x/avataaars/svg?seed=admin", PublicId = "admin_avatar", Member = members[0] },
            new() { Id = Guid.NewGuid().ToString(), Url = "https://api.dicebear.com/7.x/avataaars/svg?seed=john", PublicId = "john_avatar", Member = members[1] },
            new() { Id = Guid.NewGuid().ToString(), Url = "https://api.dicebear.com/7.x/avataaars/svg?seed=jane", PublicId = "jane_avatar", Member = members[2] },
            new() { Id = Guid.NewGuid().ToString(), Url = "https://api.dicebear.com/7.x/avataaars/svg?seed=bob", PublicId = "bob_avatar", Member = members[3] },
            new() { Id = Guid.NewGuid().ToString(), Url = "https://api.dicebear.com/7.x/avataaars/svg?seed=alice", PublicId = "alice_avatar", Member = members[4] },
        };

        await dbContext.Photos.AddRangeAsync(photos);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} photos.", photos.Count);
    }

    private static User CreateUser(string email, string name, string password)
    {
        using var hmac = new HMACSHA512();
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = name,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            PasswordSalt = hmac.Key,
            ImageUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={email.Split('@')[0]}"
        };
    }

    private static Member CreateMember(string userId, string name, string gender, string description, string city, string country, DateOnly dateOfBirth)
    {
        return new Member
        {
            Id = userId,
            Name = name,
            Gender = gender,
            Description = description,
            City = city,
            Country = country,
            DateOfBirth = dateOfBirth,
            CreatedAt = DateTime.UtcNow,
            LastActiveAt = DateTime.UtcNow,
            ImageUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={name.ToLower().Replace(" ", "")}"
        };
    }
}