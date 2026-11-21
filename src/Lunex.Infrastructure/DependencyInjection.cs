using Lunex.Application.Common.Services.Abstractions;
using Lunex.Domain.Identity.Repositories;
using Lunex.Domain.Identity.Services;
using Lunex.Domain.Member.Repositories;
using Lunex.Infrastructure.Persistance;
using Lunex.Infrastructure.Persistance.Repositories;
using Lunex.Infrastructure.Persistence;
using Lunex.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lunex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddServices();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions => npgsqlOptions
            .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Constants.Schemas.Application))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
