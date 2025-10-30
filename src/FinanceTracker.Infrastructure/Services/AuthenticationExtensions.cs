using System.Security.Claims;
using System.Text;
using FinanceTracker.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.Infrastructure.Services;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT Bearer authentication services to the service collection.
    /// This method configures the default authentication scheme to handle JWTs for standard authorization.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configuration">The application configuration for accessing JWT settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the authentication services configured.</returns>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier);

                        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                        {
                            context.Fail("Invalid token: Missing or invalid user ID claim.");
                            return;
                        }

                        var userService = context.HttpContext.RequestServices
                                             .GetRequiredService<IUserService>();

                        var userExists = await userService.ExistsByIdAsync(userId);

                        if (!userExists)
                        {
                            context.Fail("Authentication failed: User does not exist.");
                        }
                    }
                };
            });

        return services;
    }
}
