using ApiGateway.DependecyInjection.Options;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ApiGateway.RateLimiter;

public static class RateLimitExtensions
{
    public static readonly string PerUserRateLimit = nameof(PerUserRateLimit);

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = new RateLimitOptions();
        configuration.GetSection(nameof(RateLimitOptions)).Bind(rateLimitOptions);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Options 01, apply all users => Not greate
            // Options 02: apply to individual users => Recommend
            options.AddPolicy(PerUserRateLimit, httpContext =>
            {
                /*
                 * Rate limiting by IP address can be a good layer of security for unauthenticated users.
                 * Use this case => Have to put UseRateLimiter() after UseAuthentication() and UseAuthorization()
                 * var userId = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                 */

                /*
                 * We always have a user id
                 * Rate limiting by the user identifier
                 * Use this case => Have to put UseRateLimiter() after UseAuthentication() and UseAuthorization()
                 * Because to get Claims from httpContext
                 */
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)! ?? "anonymous";

                return RateLimitPartition.GetTokenBucketLimiter(userId, key =>
                    new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = rateLimitOptions.TokenLimit,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimitOptions.ReplenishmentPeriod),
                        TokensPerPeriod = rateLimitOptions.TokensPerPeriod,
                        AutoReplenishment = true,
                        QueueLimit = rateLimitOptions.QueueLimit
                    }
                );
            });
        });

        return services;
    }

    public static IEndpointConventionBuilder RequirePerUserLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting(PerUserRateLimit);
    }
}