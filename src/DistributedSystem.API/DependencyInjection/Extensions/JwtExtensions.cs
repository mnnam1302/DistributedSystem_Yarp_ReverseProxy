using DistributedSystem.API.Attributes;
using DistributedSystem.Infrastructure.DependencyInjection.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DistributedSystem.API.DependencyInjection.Extensions
{
    public static class JwtExtensions
    {
        public static void AddJwtAuthenticationAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                JwtOptions jwtOptions = new JwtOptions();
                configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);

                /**
                 * Store the JWT in the AuthenticationProperties allows you to retrieve it from the HttpContext at any time.
                 * public async Task<IActionResult> SomeAction()
                 * {
                 *  using Microsoft.AspNetCore.Authentication;
                 *  var accessToken = await HttpContext.GetTokenAsync("access_token");
                 *  //...
                 * }
                 */
                options.SaveToken = true; // Save token into AuthenticationProperties

                //var Key = Encoding.UTF8.GetBytes("7jCDPbBkeW8asPxdIc3jRddWpB7l63fh"); // On production
                var Key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // on production, set to true
                    ValidateAudience = true, // on production, set to true
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    }
                };

                options.EventsType = typeof(CustomJwtBearerEvents);
            });


            services.AddAuthorization();
            // Custom JwtBearerEvents to check token in Redis => In case, token has been revoked
            services.AddScoped<CustomJwtBearerEvents>();
        }
    }
}