using ApiGateway.DependecyInjection.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiGateway.DependecyInjection.Extensions
{
    public static class JwtExtensions
    {
        public static void AddJwtAuthenticationApiGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtOptions = new JwtOptions();
                configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);

                options.SaveToken = true; // Save token into AuthenticationProperties

                var Key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
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
                            context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");

                        return Task.CompletedTask;
                    }
                };

                //options.EventsType = typeof(CustomJwtBearerEvents);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("authPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });

            //services.AddAuthorization();

            //services.AddScoped<CustomJwtBearerEvents>()
        }
    }
}