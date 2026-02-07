using Lunex.Api;
using Lunex.Application;
using Lunex.Domain.Settings;
using Lunex.Infrastructure;

using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddWebApi(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
}

WebApplication app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseCors(CorsSettings.PolicyName);
    app.UseAuthorization();
    app.MapControllers();

    await app.RunAsync();
}