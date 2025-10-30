using FastEndpoints;
using StudentManagementAPI.Services.Interfaces;

public class DeleteEnrollmentEndpoint : EndpointWithoutRequest<object>
{
    private readonly IEnrollmentService _enrollmentService;

    public DeleteEnrollmentEndpoint(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    public override void Configure()
    {
        Delete("/api/enrollments/{studentId}/{classId}"); // FIXED: Added route parameters
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var studentId = Route<int>("studentId");
        var classId = Route<int>("classId");
        var removed = _enrollmentService.UnenrollByPair(studentId, classId);

        if (!removed)
        {
            await SendAsync(new { success = false, message = "Enrollment not found." }, 404, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Enrollment deleted successfully." }, 200, ct);
    }
}
