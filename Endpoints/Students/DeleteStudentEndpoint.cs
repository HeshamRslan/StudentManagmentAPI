using FastEndpoints;
using StudentManagementAPI.Services;

public class DeleteStudentEndpoint : EndpointWithoutRequest
{
    private readonly StudentService _studentService;

    public DeleteStudentEndpoint(StudentService studentService)
    {
        _studentService = studentService;
    }

    public override void Configure()
    {
        Delete("/api/students/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");

        var deleted = _studentService.Remove(id);

        if (!deleted)
        {
            await SendAsync(new { success = false, message = "Student not found or could not be deleted." }, 404, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Student deleted successfully." }, 200, ct);
    }
}
