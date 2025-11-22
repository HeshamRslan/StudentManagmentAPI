using FastEndpoints;
using StudentManagementAPI.Services.Infrastructure.Logging;

namespace StudentManagementAPI.Endpoints.Monitoring
{
    public class MetricsEndpoint : EndpointWithoutRequest<object>
    {
        private readonly IMetricsService _metrics;

        public MetricsEndpoint(IMetricsService metrics)
        {
            _metrics = metrics;
        }

        public override void Configure()
        {
            Get("/api/metrics");
            AllowAnonymous(); // Add authorization in production!
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var currentMetrics = _metrics.GetCurrentMetrics();

            await SendAsync(new
            {
                success = true,
                data = new
                {
                    system = new
                    {
                        memoryUsedMB = currentMetrics.MemoryUsedMB,
                        timestamp = currentMetrics.Timestamp,
                        uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\.hh\:mm\:ss")
                    },
                    requests = new
                    {
                        total = currentMetrics.TotalRequests,
                        errors = currentMetrics.ErrorCount,
                        successRate = currentMetrics.TotalRequests > 0
                            ? Math.Round((double)(currentMetrics.TotalRequests - currentMetrics.ErrorCount) / currentMetrics.TotalRequests * 100, 2)
                            : 100.0
                    }
                }
            }, 200, ct);
        }
    }
}
