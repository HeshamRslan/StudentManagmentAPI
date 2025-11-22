using FastEndpoints;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models.DTOs;

public class DeleteStudentEndpoint : EndpointWithoutRequest<ApiResponse<object>>
{
    private readonly IStudentService _studentService;

    public DeleteStudentEndpoint(IStudentService studentService)
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
            await SendAsync(new ApiResponse<object>
            {
                Success = false,
                Message = "Student not found or could not be deleted."
            }, 404, ct);
            return;
        }

        await SendAsync(new ApiResponse<object>
        {
            Success = true,
            Message = "Student deleted successfully."
        }, 200, ct);
    }
}
