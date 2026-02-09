using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Application.Accounts.Services.Implementations;
using Lunex.Application.Common.Configurations;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Common.Services.Implementations;
using Lunex.Application.Members.Services.Abstractions;
using Lunex.Application.Members.Services.Implementations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lunex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IAccountService, AccountService>();        
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
