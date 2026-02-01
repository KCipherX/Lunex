using Lunex.Application.Members;
using Lunex.Application.Services.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Lunex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        return services;
    }
}
