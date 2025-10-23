using FastEndpoints;
using StudentManagementAPI.Services;

public class DeleteClassEndpoint : EndpointWithoutRequest
{
    private readonly ClassService _classService;
    private readonly EnrollmentService _enrollmentService;

    public DeleteClassEndpoint(ClassService classService, EnrollmentService enrollmentService)
    {
        _classService = classService;
        _enrollmentService = enrollmentService;
    }

    public override void Configure()
    {
        Delete("/api/classes/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var enrolled = _enrollmentService.GetByClassId(id);
        if (enrolled.Any())
        {
            await SendAsync(new { success = false, message = "Cannot delete class with enrolled students. Unenroll them first." }, 400, ct);
            return;
        }
        // proceed to remove class


        var deleted = _classService.Remove(id);

        if (!deleted)
        {
            await SendAsync(new { success = false, message = "Class not found or could not be Removed." }, 404, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Class Removed successfully." }, 200, ct);
    }
}
