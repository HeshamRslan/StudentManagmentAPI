using FastEndpoints;
using Microsoft.Extensions.Caching.Memory;
using StudentManagementAPI.Services.Infrastructure;

namespace StudentManagementAPI.Endpoints.Cache
{
    public class GetCacheStatsEndpoint : EndpointWithoutRequest<object>
    {
        private readonly CacheConfiguration _config;

        public GetCacheStatsEndpoint(CacheConfiguration config)
        {
            _config = config;
        }

        public override void Configure()
        {
            Get("/api/cache/stats");
            AllowAnonymous(); // In production, add authorization!
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await SendAsync(new
            {
                success = true,
                cacheEnabled = _config.EnableCaching,
                expirationSettings = new
                {
                    students = $"{_config.StudentsCacheExpiration.TotalMinutes} minutes",
                    classes = $"{_config.ClassesCacheExpiration.TotalMinutes} minutes",
                    enrollments = $"{_config.EnrollmentsCacheExpiration.TotalMinutes} minutes",
                    marks = $"{_config.MarksCacheExpiration.TotalMinutes} minutes",
                    reports = $"{_config.ReportsCacheExpiration.TotalSeconds} seconds"
                },
                cacheKeys = new
                {
                    students = CacheKeys.StudentsPrefix,
                    classes = CacheKeys.ClassesPrefix,
                    enrollments = CacheKeys.EnrollmentsPrefix,
                    marks = CacheKeys.MarksPrefix,
                    reports = CacheKeys.ReportsPrefix
                }
            }, 200, ct);
        }
    }
}