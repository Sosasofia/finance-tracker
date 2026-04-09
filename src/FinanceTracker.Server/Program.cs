using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FinanceTracker.Application;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Infrastructure;
using FinanceTracker.Infrastructure.Persistence;
using FinanceTracker.Infrastructure.Services;
using FinanceTracker.Server.Middleware;
using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();

var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? "https://localhost:57861";

builder.Services.AddCors(options =>
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

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddRateLimiter(options =>
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

    options.AddPolicy("auth-limit", httpContext =>
    {
        if (HttpMethods.IsOptions(httpContext.Request.Method))
        {
            return RateLimitPartition.GetNoLimiter("preflight");
        }

        var remoteIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                       ?? httpContext.Connection.RemoteIpAddress?.ToString()
                       ?? "anonymous_auth";

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
                     ?? httpContext.Request.Headers["X-Forwarded-For"].First()
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

        var remoteIp = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                   ?? httpContext.Connection.RemoteIpAddress?.ToString()
                   ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter($"anon_{remoteIp}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 50,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });

    options.OnRejected = async (context, token) =>
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
        var ip = context.HttpContext.Connection.RemoteIpAddress;
        var user = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var endpoint = context.HttpContext.GetEndpoint();
        var attr = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();
        string activePolicy = attr?.PolicyName ?? "Global";

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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                               Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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
            new string[] {}
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});


builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

await SeedDatabaseAsync(app);

app.UseForwardedHeaders();
app.UseCors("AllowFrontend");

app.MapGet("/", () => "Hello World!").ExcludeFromDescription();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Logging.AddConsole();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();

async Task SeedDatabaseAsync(IHost app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var env = services.GetRequiredService<IWebHostEnvironment>();

        try
        {
            if (env.IsDevelopment())
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();

                await SeedInitialData.Initialize(context);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
