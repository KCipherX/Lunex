using System.Text.Json.Serialization;

using Lunex.Api.Exceptions;
using Lunex.Api.Filters;
using Lunex.Application;
using Lunex.Application.Common.Configurations;
using Lunex.Application.Common.Services.Implementations;
using Lunex.Domain.Settings;
using Lunex.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace Lunex.Api;

public static class DepencencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddApplication(configuration)
            .AddInfrastructure(configuration)
            .AddApiDocumentation()
            .AddCors(configuration)
            .AddControllersAndItsConfigurations()
            .AddExceptionHandlers()
            .AddAuthentication(configuration)
            .AddFilters();

        return services;
    }

    private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        return services
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info.Title = "Lunex API";
                    document.Info.Version = "v1";
                    document.Info.Description = """
                        Lunex API for member management and authentication.

                        ## Features
                        - **Authentication**: User registration and JWT-based login
                        - **Member Profiles**: Create, read, update member profiles with photos
                        - **Photo Management**: Upload, set main, delete photos via Cloudinary
                        - **Filtering & Pagination**: Filter members by gender, exclude current user

                        ## Authentication
                        All member endpoints require JWT Bearer token authentication.
                        Obtain token via `POST /api/account/login` or `POST /api/account/register`.

                        ## Rate Limits
                        - Photo upload: 10MB max file size
                        - Supported formats: JPG, JPEG, PNG, GIF, WebP
                        """;
                    return Task.CompletedTask;
                });
            });
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
        services.ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }

    private static IServiceCollection AddFilters(this IServiceCollection services)
    {
        services.AddScoped<LogUserActivity>();
        services.Configure<MvcOptions>(options => options.Filters
            .AddService<LogUserActivity>());

        return services;
    }
}