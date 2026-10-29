using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

namespace FinanceTracker.Server.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendUrl = configuration["FRONTEND_URL"] ?? "https://localhost:57861";

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(frontendUrl)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .WithExposedHeaders("Retry-After")
                      .SetPreflightMaxAge(TimeSpan.FromHours(1));
            });
        });

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        AddSwagger(services);
        AddCustomRateLimiting(services);

        return services;
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Add JWT Bearer Authorization
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your valid token in the text input below.\r\n\r\nFinal Token Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                        new string[] { }
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
    }

    private static void AddCustomRateLimiting(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "global-limit",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 300,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 15
                    }));

            options.AddFixedWindowLimiter("session-limit", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
            });
            options.AddPolicy("auth-limit", httpContext =>
            {
                if (HttpMethods.IsOptions(httpContext.Request.Method))
                {
                    return RateLimitPartition.GetNoLimiter("preflight");
                }

                var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous_auth";

                return RateLimitPartition.GetFixedWindowLimiter($"auth_{remoteIp}", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy("download-limit", httpContext =>
            {
                if (HttpMethods.IsOptions(httpContext.Request.Method))
                {
                    return RateLimitPartition.GetNoLimiter("preflight");
                }

                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                     ?? httpContext.Connection.RemoteIpAddress?.ToString()
                                     ?? "anon_downloader";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"dl_{userId}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            options.AddPolicy("fixed", httpContext =>
            {
                if (HttpMethods.IsOptions(httpContext.Request.Method))
                {
                    return RateLimitPartition.GetNoLimiter("preflight");
                }

                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"user_{userId}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 200,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                }

                var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                return RateLimitPartition.GetFixedWindowLimiter($"anon_{remoteIp}", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 50,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddFixedWindowLimiter("receipt-scanner-policy", opt =>
            {
                opt.PermitLimit = 10;
                opt.Window = TimeSpan.FromHours(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
            options.OnRejected = async (context, token) =>
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
                var ip = context.HttpContext.Connection.RemoteIpAddress;
                var user = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var endpoint = context.HttpContext.GetEndpoint();
                var attr = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();
                string activePolicy = attr?.PolicyName ?? "Global";

                FinanceTracker.Server.Metrics.ApiMetrics.RateLimitExceededCounter.Add(1,
                    new KeyValuePair<string, object?>("policy", activePolicy));

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                int retryAfter = 60;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterSpan))
                {
                    retryAfter = (int)retryAfterSpan.TotalSeconds;
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();
                }

                string title = "Too many requests.";
                string detail = "You have exceeded your quota. Please try again later.";

                if (activePolicy == "auth-limit")
                {
                    title = "Security: Too many login attempts.";
                    detail = "For your protection, login is temporarily disabled for your IP.";
                    logger?.LogWarning("Potential Brute Force from IP: {IP}. Policy: {Policy}", ip, activePolicy);
                }
                else if (activePolicy == "download-limit")
                {
                    title = "Export Limit Reached";
                    detail = "Please wait a minute before generating another report.";
                    logger?.LogWarning("Download limit hit by User: {User}, IP: {IP}", user, ip);
                }
                else if (activePolicy == "receipt-scanner-policy")
                {
                    title = "Scan Limit Reached";
                    detail = "You have reached your limit of 10 receipt scans per hour. Please try again later or enter the expense manually.";
                    logger?.LogWarning("Receipt scan limit hit by User: {User}, IP: {IP}", user, ip);
                }
                else
                {
                    logger?.LogWarning("Rate limit exceeded for User: {User}, IP: {IP}. Policy: {Policy}", user, ip, activePolicy);
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    status = StatusCodes.Status429TooManyRequests,
                    error = title,
                    message = detail,
                    retryAfterSeconds = retryAfter
                }, token);
            };
        });
    }
}

