using FastEndpoints;
using Serilog;

namespace StudentManagementAPI.Endpoints.Monitoring
{
    public class TestSlowRequest
    {
        public int DelayMs { get; set; } = 2000;
    }

    public class TestSlowEndpoint : Endpoint<TestSlowRequest, object>
    {
        public override void Configure()
        {
            Get("/api/test/slow");
            AllowAnonymous();
        }

        public override async Task HandleAsync(TestSlowRequest req, CancellationToken ct)
        {
            Log.Information("Test slow endpoint called with delay: {DelayMs}ms", req.DelayMs);

            await Task.Delay(req.DelayMs, ct);

            await SendAsync(new
            {
                success = true,
                message = $"Completed after {req.DelayMs}ms delay",
                timestamp = DateTime.UtcNow
            }, 200, ct);
        }
    }
}