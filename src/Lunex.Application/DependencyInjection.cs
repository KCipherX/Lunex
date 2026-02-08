using Lunex.Application.Accounts.Services.Abstractions;
using Lunex.Application.Accounts.Services.Implementations;
using Lunex.Application.Members.Services.Abstractions;
using Lunex.Application.Members.Services.Implementations;

using Microsoft.Extensions.DependencyInjection;

namespace Lunex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IAccountService, AccountService>();
        return services;
    }
}
