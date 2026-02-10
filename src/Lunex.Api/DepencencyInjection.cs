using System.Text.Json.Serialization;

using Lunex.Api.Exceptions;
using Lunex.Application.Common.Configurations;
using Lunex.Application.Common.Services.Abstractions;
using Lunex.Application.Common.Services.Implementations;
using Lunex.Domain.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Lunex.Api;

public static class DepencencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddOpenApi()
            .AddCors(configuration)
            .AddControllersAndItsConfigurations()
            .AddExceptionHandlers()
            .AddAuthentication(configuration);

        return services;
    }
    
    private static IServiceCollection AddCors(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var corsSettings = configuration.GetRequiredSection(CorsSettings.SectionName)
            .Get<CorsSettings>()!;

        services.AddCors(corsOptions => corsOptions.AddPolicy(
            name: CorsSettings.PolicyName,
            configurePolicy: corsPolicyBuilder => corsPolicyBuilder
                .WithOrigins(corsSettings.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

        return services;
    }

    private static IServiceCollection AddControllersAndItsConfigurations(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(
            configure =>
            {
                configure.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    private static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }
}