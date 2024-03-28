using ApiGateway.DependecyInjection.Extensions;
using ApiGateway.RateLimiter;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Rate Limiter
builder.Services.AddRateLimiting(builder.Configuration);

// Add JWT
builder.Services.AddJwtAuthenticationApiGateway(builder.Configuration);

// Add Yarp Reverse Proxy
builder.Services.AddYarpReverseProxyApiGateway(builder.Configuration);

builder.Services.AddServicesApiGateway();

builder.Services.AddRedisApiGateway(builder.Configuration);

var app = builder.Build();


//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

//app.MapReverseProxy();
//app.MapReverseProxy().RequireRateLimiting(RateLimitExtensions.PerUserRateLimit);
app.MapReverseProxy().RequirePerUserLimit();


app.Run();
