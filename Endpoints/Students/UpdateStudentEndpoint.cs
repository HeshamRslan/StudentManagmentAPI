using FastEndpoints;
using StudentManagementAPI.Mappings;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models.DTOs;

public class UpdateStudentRequest
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class UpdateStudentEndpoint : Endpoint<UpdateStudentRequest, ApiResponse<StudentResponse>>
{
    private readonly IStudentService _studentService;

    public UpdateStudentEndpoint(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public override void Configure()
    {
        Put("/api/students/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateStudentRequest req, CancellationToken ct)
    {
        var id = Route<int>("id");
        var existing = _studentService.GetById(id);

        if (existing == null)
        {
            await SendAsync(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = "Student not found."
            }, 404, ct);
            return;
        }

        existing.FirstName = req.FirstName.Trim();
        existing.LastName = req.LastName.Trim();
        existing.Age = req.Age;

        var updated = _studentService.Update(existing);

        if (!updated)
        {
            await SendAsync(new ApiResponse<StudentResponse>
            {
                Success = false,
                Message = "Failed to update student."
            }, 500, ct);
            return;
        }

        await SendAsync(new ApiResponse<StudentResponse>
        {
            Success = true,
            Message = "Student updated successfully.",
            Data = existing.ToResponse()
        }, 200, ct);
    }
}