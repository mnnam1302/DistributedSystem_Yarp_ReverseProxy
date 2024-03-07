using ApiGateway.DependecyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddJwtAuthenticationApiGateway(builder.Configuration);

builder.Services.AddYarpReverseProxyApiGateway(builder.Configuration);

builder.Services.AddServicesApiGateway();

builder.Services.AddRedisApiGateway(builder.Configuration);

var app = builder.Build();


//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapReverseProxy();

app.Run();
