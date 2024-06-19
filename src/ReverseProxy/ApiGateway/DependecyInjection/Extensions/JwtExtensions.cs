using System.Text;
using ApiGateway.Attributes;
using ApiGateway.DependecyInjection.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApiGateway.DependecyInjection.Extensions;

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

            // Have to get Email from token
            // Get AuthValue from redis to get PublicKey to verify token
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
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
                },
            };

            options.EventsType = typeof(CustomJwtBearerEvents);
        });

        services.AddAuthorization(options
            => options.AddPolicy("authPolicy", policy => policy.RequireAuthenticatedUser()));

        services.AddScoped<CustomJwtBearerEvents>();
    }
}
