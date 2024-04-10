using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace PaymentGateway.Api.Extensions;

public static class DependencyInjection
{
    public static void AddTelemetry(this WebApplicationBuilder builder)
    {
        var zipkinEndpoint = builder.Configuration.GetValue<string>("Telemetry:ZipkinEndpoint")!;

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService("PaymentGateway.Api Logging"))
                .AddConsoleExporter();
        });

        Sdk.CreateTracerProviderBuilder()
            .ConfigureResource(config => config.AddService("PaymentGateway.Api HTTP"))
            .AddHttpClientInstrumentation()
            .AddZipkinExporter(options =>
            {
                options.Endpoint = new(zipkinEndpoint);
            })
            .AddConsoleExporter()
            .Build();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .ConfigureResource(config => config.AddService("PaymentGateway.Api Tracing"))
                .AddAspNetCoreInstrumentation()
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new(zipkinEndpoint);
                })
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .ConfigureResource(config => config.AddService("PaymentGateway.Api Metrics"))
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());
    }
}