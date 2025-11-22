using FastEndpoints;
using StudentManagementAPI.Mappings;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models;
using StudentManagmentAPI.Models.DTOs;

public class CreateStudentRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class CreateStudentEndpoint : Endpoint<CreateStudentRequest, ApiResponse<StudentResponse>>
{
    private readonly IStudentService _studentService;

    public CreateStudentEndpoint(IStudentService studentService)
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
            await SendAsync(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = "First name and last name are required."
            }, 400, ct);
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
            await SendAsync(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = "Failed to create student."
            }, 500, ct);
            return;
        }

        await SendAsync(new ApiResponse<StudentResponse>
        {
            Success = true,
            Message = "Student created successfully.",
            Data = student.ToResponse()
        }, 201, ct);
    }
}