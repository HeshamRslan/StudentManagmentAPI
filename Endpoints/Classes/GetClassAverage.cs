using FastEndpoints;
using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;

public class GetClassAverageEndpoint : EndpointWithoutRequest<object>
{
    private readonly IClassService _classService;
    private readonly MarkService _markService;

    public GetClassAverageEndpoint(IClassService classService, MarkService markService)
    {
        _classService = classService;
        _markService = markService;
    }

    public override void Configure()
    {
        Get("/api/classes/{classId}/average-marks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var classId = Route<int>("classId");

        var cls = _classService.GetById(classId);
        if (cls == null)
        {
            await SendAsync(new { success = false, message = "Class not found." }, 404, ct);
            return;
        }

        var marks = _markService.GetAll().Where(m => m.ClassId == classId).ToList();
        if (marks.Count == 0)
        {
            await SendAsync(new { success = false, message = "No marks found for this class." }, 404, ct);
            return;
        }

        var avg = marks.Average(m => m.TotalMark);

        await SendAsync(new
        {
            success = true,
            classInfo = new { cls.Id, cls.Name, cls.Teacher },
            totalStudents = marks.Count,
            averageMark = Math.Round(avg, 2)
        }, 200, ct);
    }
}
