using Authorization.API.DependencyInjection.Extensions;
using Authorization.Persistence.DependencyInjection.Extensions;
using Authorization.API.Middleware;
using Carter;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Serilog;
using Authorization.Infrastructure.DependecyInjection.Extensions;
using Authorization.Application.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Serilog
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Host.UseSerilog();

// Add Carter
builder.Services.AddCarter();

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

builder.Services.AddMediatRApplication();
builder.Services.AddAutoMapperApplication();

builder.Services.ConfigureSqlServerRetryOptionsPersistence(builder.Configuration.GetSection("SqlServerRetryOptions"));
builder.Services.AddSqlPersistence(builder.Configuration);
builder.Services.AddRepositoryPersistence();


builder.Services.AddServicesInfrastructure();


builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapCarter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    app.UseSwaggerAPI();

//app.UseHttpsRedirection();
//app.UseAuthorization();

app.Run();
