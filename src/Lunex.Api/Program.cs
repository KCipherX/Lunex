using Lunex.Api;
using Lunex.Application;
using Lunex.Domain.Settings;
using Lunex.Infrastructure;
using Lunex.Infrastructure.Persistance.Seeder;

using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebApi(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // Add database seeding
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DatabaseSeeder.SeedAsync(app.Services, logger);
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors(CorsSettings.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();