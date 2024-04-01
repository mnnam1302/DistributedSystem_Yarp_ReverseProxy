using Carter;
using DistributedSystem.API.DependencyInjection.Extensions;
using DistributedSystem.API.Middleware;
using DistributedSystem.Application.DependencyInjection.Extensions;
using DistributedSystem.Infrastructure.DependencyInjection.Extensions;
using DistributedSystem.Persistence.DependencyInjection.Extensions;
using DistributedSystem.Persistence.DependencyInjection.Options;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add configuration


Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

// Add Serilog
builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();


// Add Carter module
builder.Services.AddCarter();

// Read more: Use in case for Controller API DistributedSystem.Presentation
//builder.
//    Services
//    .AddControllers()
//    .AddApplicationPart(DistributedSystem.Persistence.AssemblyReference.Assembly);

// Add Swagger
builder.Services
    .AddSwaggerGenNewtonsoftSupport()
    .AddFluentValidationRulesToSwagger()
    .AddEndpointsApiExplorer()
    .AddSwaggerAPI();

// Add API versioning
builder.Services
    .AddApiVersioning(options => options.ReportApiVersions = true)
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// Add Jwt Authentication => After, app.UseAuthentication(); app.UseAuthorization();
//builder.Services.AddJwtAuthenticationAPI(builder.Configuration); => VALIDATION AT SERVER ApiGateway


builder.Services.AddMediatRApplication();
builder.Services.AddAutoMapperApplication();

// Configure masstransit rabbitmq
builder.Services.AddMasstransitRabbitMQInfrastructure(builder.Configuration);
builder.Services.AddQuartzInfrastructure();
builder.Services.AddMediatRInfrastructure();
builder.Services.AddServicesInfrastructure();
builder.Services.AddRedisInfrastructure(builder.Configuration);


// Configure Options and SQL =>  remember mapcarter
// Pass Configuration good - builder.Configuration.GetSection(nameof(SqlServerRetryOptions))
// Not hard code at ConfigureSqlServerRetryOptions at Persistence ** My ERROR
builder.Services.AddInterceptorPersistence();
builder.Services.ConfigureSqlServerRetryOptionsPersistence(builder.Configuration.GetSection(nameof(SqlServerRetryOptions)));
builder.Services.AddSqlPersistence();
builder.Services.AddRepositoryPersistence();

// Add OpenTelemetry
builder.AddOpenTelemetryInfrastructure();

// Add Middleware => Remember use middleware
builder.Services.AddTransient<ExceptionHandlingMiddleware>();


var app = builder.Build();

// Using middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection(); // => Use in case for production

//app.UseAuthentication(); // This to need added before UseAuthorization
//app.UseAuthorization();

//app.MapControllers(); // Use in case Controller API

// Add API endpoint with Carter module
app.MapCarter(); // Must be after authenticatio and authorization


// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
    app.UseSwaggerAPI(); // => After MapCarter => Show Version


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
