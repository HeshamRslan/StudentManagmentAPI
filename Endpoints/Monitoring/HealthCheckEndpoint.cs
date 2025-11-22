using FastEndpoints;
using StudentManagementAPI.Services.Infrastructure.Logging;
using Serilog;

namespace StudentManagementAPI.Endpoints.Monitoring
{
    public class HealthCheckEndpoint : EndpointWithoutRequest<object>
    {
        private readonly IMetricsService _metrics;

        public HealthCheckEndpoint(IMetricsService metrics)
        {
            _metrics = metrics;
        }

        public override void Configure()
        {
            Get("/api/health");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var metrics = _metrics.GetCurrentMetrics();

            var health = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss"),
                metrics = new
                {
                    memoryUsedMB = metrics.MemoryUsedMB,
                    totalRequests = metrics.TotalRequests,
                    errorCount = metrics.ErrorCount,
                    errorRate = metrics.TotalRequests > 0
                        ? Math.Round((double)metrics.ErrorCount / metrics.TotalRequests * 100, 2)
                        : 0
                },
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            Log.Information("Health check requested: Memory={MemoryMB}MB, Requests={Requests}, Errors={Errors}",
                metrics.MemoryUsedMB, metrics.TotalRequests, metrics.ErrorCount);

            await SendAsync(health, 200, ct);
        }
    }
}
