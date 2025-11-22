using FastEndpoints;
using Serilog;

namespace StudentManagementAPI.Endpoints.Monitoring
{
    public class TestErrorEndpoint : EndpointWithoutRequest<object>
    {
        public override void Configure()
        {
            Get("/api/test/error");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            Log.Warning("Test error endpoint called - simulating exception");
            throw new InvalidOperationException("This is a test exception to verify error logging");
        }
    }
}
