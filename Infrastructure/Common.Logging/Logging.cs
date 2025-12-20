using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging;

public static class Logging
{
    public static Action<HostBuilderContext, LoggerConfiguration> cfg =>
        (context, loggerConfiguration) =>
        {
            var env = context.HostingEnvironment;
            loggerConfiguration.MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                .Enrich.WithExceptionDetails()
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console();
            if (context.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Override("Catalog", Serilog.Events.LogEventLevel.Debug);
                loggerConfiguration.MinimumLevel.Override("Basket", Serilog.Events.LogEventLevel.Debug);
                loggerConfiguration.MinimumLevel.Override("Discount", Serilog.Events.LogEventLevel.Debug);
                loggerConfiguration.MinimumLevel.Override("Ordering", Serilog.Events.LogEventLevel.Debug);
            }
            // Elasticsearch configuration
            var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                loggerConfiguration.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                        IndexFormat = "ecommerce-logs-{0:yyyy.MM.dd}",
                        MinimumLogEventLevel = LogEventLevel.Debug
                    }
                );
            }

        };
}