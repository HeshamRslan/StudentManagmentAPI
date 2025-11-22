using FastEndpoints;
using StudentManagementAPI.Mappings;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models.DTOs;

public class GetAllStudentsEndpoint : EndpointWithoutRequest<ApiResponse<List<StudentResponse>>>
{
    private readonly IStudentService _studentService;

    public GetAllStudentsEndpoint(IStudentService studentService)
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
        var students = _studentService.GetAll().ToList();

        await SendAsync(new ApiResponse<List<StudentResponse>>
        {
            Success = true,
            Message = "Students retrieved successfully.",
            Data = students.Select(s => s.ToResponse()).ToList()
        }, 200, ct);
    }
}