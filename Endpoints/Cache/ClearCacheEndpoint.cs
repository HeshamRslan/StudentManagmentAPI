using FastEndpoints;
using StudentManagementAPI.Services.Infrastructure;

namespace StudentManagementAPI.Endpoints.Cache
{
    public class ClearCacheRequest
    {
        public string? Prefix { get; set; }
    }

    public class ClearCacheEndpoint : Endpoint<ClearCacheRequest, object>
    {
        private readonly ICacheService _cache;

        public ClearCacheEndpoint(ICacheService cache)
        {
            _cache = cache;
        }

        public override void Configure()
        {
            Post("/api/cache/clear");
            AllowAnonymous(); 
        }

        public override async Task HandleAsync(ClearCacheRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Prefix))
            {
                _cache.Clear();
                await SendAsync(new
                {
                    success = true,
                    message = "All cache cleared successfully."
                }, 200, ct);
            }
            else
            {
                _cache.RemoveByPrefix(req.Prefix);
                await SendAsync(new
                {
                    success = true,
                    message = $"Cache cleared for prefix: {req.Prefix}"
                }, 200, ct);
            }
        }
    }
}
