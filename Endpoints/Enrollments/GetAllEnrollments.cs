using FastEndpoints;
using StudentManagementAPI.Services;

public class GetAllEnrollmentsRequest
{
    public int? StudentId { get; set; }
    public int? ClassId { get; set; }
}

public class GetAllEnrollmentsEndpoint : Endpoint<GetAllEnrollmentsRequest, object>
{
    private readonly EnrollmentService _enrollmentService;

    public GetAllEnrollmentsEndpoint(EnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    public override void Configure()
    {
        Get("/api/enrollments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllEnrollmentsRequest req, CancellationToken ct)
    {
        var all = _enrollmentService.GetAll();

        if (req.StudentId.HasValue)
            all = all.Where(e => e.StudentId == req.StudentId.Value);

        if (req.ClassId.HasValue)
            all = all.Where(e => e.ClassId == req.ClassId.Value);

        await SendAsync(new { success = true, total = all.Count(), data = all }, 200, ct);
    }
}
