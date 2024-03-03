var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxyQuery"))
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxyCommand"));

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
