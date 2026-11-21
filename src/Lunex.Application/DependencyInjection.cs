using Lunex.Application.Common.Configurations;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Common.Services.Implementations;
using Lunex.Application.Identity.Services;
using Lunex.Application.Member.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lunex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IMemberService, MemberService>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
