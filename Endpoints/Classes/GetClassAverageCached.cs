using FastEndpoints;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Infrastructure;

public class GetClassAverageCachedEndpoint : EndpointWithoutRequest<object>
{
    private readonly IClassService _classService;
    private readonly IMarkService _markService;
    private readonly ICacheService _cache;
    private readonly CacheConfiguration _cacheConfig;

    public GetClassAverageCachedEndpoint(
        IClassService classService,
        IMarkService markService,
        ICacheService cache,
        CacheConfiguration cacheConfig)
    {
        _classService = classService;
        _markService = markService;
        _cache = cache;
        _cacheConfig = cacheConfig;
    }

    public override void Configure()
    {
        Get("/api/classes/{classId}/average-marks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var classId = Route<int>("classId");

        // Check cache first
        var cacheKey = CacheKeys.ClassAverage(classId);

        var result = await _cache.GetOrSetAsync(
            cacheKey,
            async () => await CalculateClassAverageAsync(classId),
            _cacheConfig.MarksCacheExpiration
        );

        if (result == null)
        {
            await SendAsync(new { success = false, message = "Class not found or no marks available." }, 404, ct);
            return;
        }

        await SendAsync(result, 200, ct);
    }

    private async Task<object?> CalculateClassAverageAsync(int classId)
    {
        var cls = _classService.GetById(classId);
        if (cls == null) return null;

        var marks = _markService.GetByClass(classId).ToList();
        if (marks.Count == 0) return null;

        var avg = marks.Average(m => m.TotalMark);

        return await Task.FromResult(new
        {
            success = true,
            classInfo = new { cls.Id, cls.Name, cls.Teacher },
            totalStudents = marks.Count,
            averageMark = Math.Round(avg, 2),
            cachedAt = DateTime.UtcNow,
            expiresIn = $"{_cacheConfig.MarksCacheExpiration.TotalMinutes} minutes"
        });
    }
}