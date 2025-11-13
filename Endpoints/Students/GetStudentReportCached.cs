using FastEndpoints;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Infrastructure;

public class GetStudentReportCachedEndpoint : EndpointWithoutRequest<object>
{
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;
    private readonly IMarkService _markService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICacheService _cache;
    private readonly CacheConfiguration _cacheConfig;

    public GetStudentReportCachedEndpoint(
        IStudentService studentService,
        IClassService classService,
        IMarkService markService,
        IEnrollmentService enrollmentService,
        ICacheService cache,
        CacheConfiguration cacheConfig)
    {
        _studentService = studentService;
        _classService = classService;
        _markService = markService;
        _enrollmentService = enrollmentService;
        _cache = cache;
        _cacheConfig = cacheConfig;
    }

    public override void Configure()
    {
        Get("/api/students/{studentId}/report");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var studentId = Route<int>("studentId");

        // Check cache first
        var cacheKey = CacheKeys.StudentReport(studentId);

        var result = await _cache.GetOrSetAsync(
            cacheKey,
            async () => await GenerateStudentReportAsync(studentId),
            _cacheConfig.ReportsCacheExpiration
        );

        if (result == null)
        {
            await SendAsync(new { success = false, message = "Student not found." }, 404, ct);
            return;
        }

        await SendAsync(result, 200, ct);
    }

    private async Task<object?> GenerateStudentReportAsync(int studentId)
    {
        var student = _studentService.GetById(studentId);
        if (student == null) return null;

        var marks = _markService.GetByStudent(studentId).ToList();
        var enrolledClassIds = _enrollmentService.GetByStudentId(studentId)
            .Select(e => e.ClassId)
            .Distinct()
            .ToList();

        var classes = new List<object>();

        foreach (var classId in enrolledClassIds)
        {
            var cls = _classService.GetById(classId);
            if (cls == null) continue;

            var classMarksAll = _markService.GetByClass(classId).ToList();
            var studentMarks = classMarksAll
                .Where(m => m.StudentId == studentId)
                .OrderBy(m => m.RecordedAt)
                .ToList();

            decimal studentAvgInClass = studentMarks.Any()
                ? studentMarks.Average(m => m.TotalMark)
                : 0m;

            var rankingList = classMarksAll
                .GroupBy(m => m.StudentId)
                .Select(g => new { StudentId = g.Key, Avg = g.Average(m => m.TotalMark) })
                .OrderByDescending(x => x.Avg)
                .ToList();

            var rankIndex = rankingList.FindIndex(x => x.StudentId == studentId);
            int rank = rankIndex >= 0 ? rankIndex + 1 : rankingList.Count + 1;
            int totalStudentsInClass = rankingList.Count;

            var grade = GradeFromAvg(studentAvgInClass);
            var trend = CalculateTrend(studentMarks);

            classes.Add(new
            {
                id = cls.Id,
                name = cls.Name,
                teacher = cls.Teacher,
                studentAverage = Math.Round(studentAvgInClass, 2),
                grade,
                rank = $"{Ordinal(rank)} out of {totalStudentsInClass}",
                trend,
                marks = studentMarks.Select(m => new
                {
                    m.ExamMark,
                    m.AssignmentMark,
                    m.TotalMark,
                    m.RecordedAt
                }).ToList()
            });
        }

        var overallAverage = marks.Any()
            ? Math.Round(marks.Average(m => m.TotalMark), 2)
            : 0m;

        return await Task.FromResult(new
        {
            success = true,
            student = new { student.Id, student.FirstName, student.LastName },
            classes,
            overallAverage,
            cachedAt = DateTime.UtcNow,
            expiresIn = $"{_cacheConfig.ReportsCacheExpiration.TotalSeconds} seconds"
        });
    }

    private string GradeFromAvg(decimal avg) =>
        avg >= 90 ? "A" :
        avg >= 80 ? "B" :
        avg >= 70 ? "C" :
        avg >= 60 ? "D" : "F";

    private string Ordinal(int n)
    {
        if (n % 100 >= 11 && n % 100 <= 13) return n + "th";
        return (n % 10) switch
        {
            1 => n + "st",
            2 => n + "nd",
            3 => n + "rd",
            _ => n + "th"
        };
    }

    private string CalculateTrend(List<StudentManagmentAPI.Models.Mark> studentMarks)
    {
        if (studentMarks.Count < 2) return "stable";

        var half = studentMarks.Count / 2;
        var firstAvg = studentMarks.Take(half).Any()
            ? studentMarks.Take(half).Average(m => m.TotalMark)
            : studentMarks.First().TotalMark;
        var lastAvg = studentMarks.Skip(half).Any()
            ? studentMarks.Skip(half).Average(m => m.TotalMark)
            : studentMarks.Last().TotalMark;

        if (lastAvg > firstAvg + 1) return "improving";
        if (lastAvg < firstAvg - 1) return "declining";
        return "stable";
    }
}