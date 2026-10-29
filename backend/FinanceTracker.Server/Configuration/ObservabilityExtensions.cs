using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace FinanceTracker.Server.Configuration;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var serviceName = "FinanceTracker.Api";

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", Serilog.Events.LogEventLevel.Fatal)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.OpenTelemetry(options =>
                 {
                     if (!string.IsNullOrEmpty(otlpEndpoint))
                     {
                         options.Endpoint = otlpEndpoint;
                     }
                     options.ResourceAttributes = new Dictionary<string, object>
                     {
                         ["service.name"] = "FinanceTracker.Api"
                     };
                 });
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName,
                    serviceVersion: "1.0.0",
                    serviceInstanceId: "local-dev"))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddSource("FinanceTracker.Application")
                    .AddOtlpExporter(options =>
                    {
                        if (!string.IsNullOrEmpty(otlpEndpoint))
                        {
                            options.Endpoint = new Uri(otlpEndpoint);
                        }
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(FinanceTracker.Application.Common.Metrics.ApplicationMetrics.MeterName)
                    .AddMeter(FinanceTracker.Server.Metrics.ApiMetrics.MeterName)
                    .AddOtlpExporter();
            });

        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
            {
                options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
            });
        }

        return builder;
    }
}
