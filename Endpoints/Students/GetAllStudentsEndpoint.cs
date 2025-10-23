using FastEndpoints;
using StudentManagementAPI.Services;

public class GetAllStudentsEndpoint : EndpointWithoutRequest
{
    private readonly StudentService _studentService;

    public GetAllStudentsEndpoint(StudentService studentService)
    {
        _studentService = studentService;
    }

    public override void Configure()
    {
        Get("/api/students");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var students = _studentService.GetAll();
        await SendAsync(students, 200, ct);
    }
}
