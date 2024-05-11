using Carter;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Query.API.DependencyInjection.Extensions;
using Query.API.Middleware;
using Query.Application.DependencyInjection.Extensions;
using Query.Infrastructure.DependencyInjection.Extensions;
using Query.Persistence.DependencyInjection.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Query")
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder.Host.ConfigureServices((context, servies) =>
{
    // Carter
    builder.Services.AddCarter();

    // Swagger
    builder.Services
        .AddSwaggerGenNewtonsoftSupport()
        .AddFluentValidationRulesToSwagger()
        .AddEndpointsApiExplorer()
        .AddSwaggerAPI();

    // API versioning
    builder.Services
        .AddApiVersioning(options => options.ReportApiVersions = true)
        .AddApiExplorer(options =>
        {
            // Read more to understand Format: https://github.com/dotnet/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    builder.Services.AddMediatRApplication();

    builder.Services.AddServicesInfrastructure(context.Configuration);
    builder.Services.AddMasstransitRabbitMQInfrastructure(context.Configuration);

    builder.Services.AddServicesPersistence();

    builder.Services.AddOpenTelemetryInfrastructure(context.Configuration);

    // Global Exception Handler
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();

});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();
//app.UseAuthorization();

app.MapCarter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwaggerAPI(); // After MapCarter => Show Version
}

try
{
    await app.RunAsync();
    Log.Information("Stop cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}

public partial class Program { }
