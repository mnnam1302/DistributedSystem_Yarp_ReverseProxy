namespace ApiGateway.DependecyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddYarpReverseProxyApiGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddReverseProxy()
                 .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        }
    }
}
