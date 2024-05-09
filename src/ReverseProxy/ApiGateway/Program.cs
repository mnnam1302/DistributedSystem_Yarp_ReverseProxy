using ApiGateway.DependecyInjection.Extensions;
using ApiGateway.RateLimiter;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Addd Serilog
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder.Host.ConfigureServices((context, services) =>
{
    //Add services
    builder.Host.ConfigureServices((context, services) =>
    {
        // Add Rate Limiter
        builder.Services.AddRateLimiting(context.Configuration);

        // Add JWT Bearer
        builder.Services.AddJwtAuthenticationApiGateway(context.Configuration);

        // Add Yarp Reverse Proxy
        builder.Services.AddYarpReverseProxyApiGateway(context.Configuration);

        builder.Services.AddServicesApiGateway();
        builder.Services.AddRedisApiGateway(context.Configuration);

        // Add OpenTelemetry
        //builder.AddOpenTelemetryInfrastructure();

        services.AddHttpLogging(options
            => options.LoggingFields = HttpLoggingFields.All);
    });
});

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseRateLimiter();

//app.MapReverseProxy();
//app.MapReverseProxy().RequireRateLimiting(RateLimitExtensions.PerUserRateLimit);
app.MapReverseProxy().RequirePerUserLimit();


try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}

public partial class Program { }