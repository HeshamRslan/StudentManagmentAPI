using FastEndpoints;
using StudentManagementAPI.Services;
using StudentManagmentAPI.Models;

public class CreateStudentRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class CreateStudentEndpoint : Endpoint<CreateStudentRequest>
{
    private readonly StudentService _studentService;

    public CreateStudentEndpoint(StudentService studentService)
    {
        _studentService = studentService;
    }

    public override void Configure()
    {
        Post("/api/students");
        AllowAnonymous();
        Validator<CreateStudentRequestValidator>();
    }

    public override async Task HandleAsync(CreateStudentRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.FirstName) || string.IsNullOrWhiteSpace(req.LastName))
        {
            await SendAsync(new { success = false, message = "First name and last name are required." }, 400, ct);
            return;
        }

        var student = new Student
        {
            FirstName = req.FirstName.Trim(),
            LastName = req.LastName.Trim(),
            Age = req.Age
        };

        var added = _studentService.Add(student);

        if (!added)
        {
            await SendAsync(new { success = false, message = "Failed to create student." }, 500, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Student created successfully.", student }, 201, ct);
    }
}
