using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using ILogger = Serilog.ILogger;

namespace StudentManagementAPI.Services.Infrastructure.Logging
{
    public static class LoggingConfiguration
    {
        public static ILogger CreateLogger(IConfiguration configuration)
        {
            var logPath = configuration["Logging:FilePath"] ?? "Logs/app-.log";
            var jsonLogPath = configuration["Logging:JsonFilePath"] ?? "Logs/app-.json";
            var retentionDays = configuration.GetValue<int>("Logging:RetentionDays", 30);

            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)

                // Enrichers for context
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", "StudentManagementAPI")

                // Console sink (human-readable in development)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")

                // File sink (human-readable logs)
                .WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: retentionDays,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}")

                // JSON file sink (structured logs for parsing)
                .WriteTo.File(
                    new CompactJsonFormatter(),
                    path: jsonLogPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: retentionDays)

                .CreateLogger();
        }
    }
}
