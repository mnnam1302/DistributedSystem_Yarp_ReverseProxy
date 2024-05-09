using Authorization.API.DependencyInjection.Extensions;
using Authorization.API.Middleware;
using Authorization.Application.DependencyInjection.Extensions;
using Authorization.Infrastructure.DependecyInjection.Extensions;
using Authorization.Persistence.DependencyInjection.Extensions;
using Carter;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Serilog
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Authorization")
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

builder.Host.ConfigureServices((context, services) =>
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
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    // builder.Services.AddJwtAuthenticationAPI(builder.Configuration); //=> VALIDATION AT SERVER ApiGateway

    // Application
    builder.Services.AddMediatRApplication();
    builder.Services.AddAutoMapperApplication();

    // Persistence
    builder.Services.ConfigureSqlServerRetryOptionsPersistence(builder.Configuration.GetSection("SqlServerRetryOptions"));
    builder.Services.AddSqlPersistence(builder.Configuration);
    builder.Services.AddRepositoryPersistence();

    // Infrastructure
    //builder.AddOpenTelemetryInfrastructure();
    builder.Services.AddServicesInfrastructure();
    builder.Services.AddRedisInfrastructure(builder.Configuration);

    // Add Custom Middleware
    builder.Services.AddTransient<ExceptionHandlingMiddleware>();
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapCarter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwaggerAPI();
}

//app.UseHttpsRedirection();
// Should add Authentication and Authorization herre. Let's check again
 //app.UseAuthentication();
 //app.UseAuthorization();

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
