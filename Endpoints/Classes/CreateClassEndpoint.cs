using FastEndpoints;
using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models.DTOs;

public class CreateClassEndpoint : Endpoint<CreateClassRequest, ApiResponse<ClassResponse>>
{
    private readonly IClassService _classService;
    private readonly EnrollmentService _enrollmentService; // optional for counts

    public CreateClassEndpoint(IClassService classService, EnrollmentService enrollmentService)
    {
        _classService = classService;
        _enrollmentService = enrollmentService;
    }

    public override void Configure()
    {
        Post("/api/classes");
        AllowAnonymous();
        Validator<CreateClassRequestValidator>();
    }

    public override async Task HandleAsync(CreateClassRequest req, CancellationToken ct)
    {
        var newClass = req.ToModel();

        var result = _classService.Create(newClass);

        if (!result.Success)
        {
            await SendAsync(new ApiResponse<ClassResponse>
            {
                Success = false,
                Message = result.Error ?? "Failed to create class."
            }, 500, ct);
            return;
        }

        await SendAsync(new ApiResponse<ClassResponse>
        {
            Success = true,
            Message = "Class created successfully.",
            Data = result.Created?.ToResponse()
        }, 201, ct);
    }
}
