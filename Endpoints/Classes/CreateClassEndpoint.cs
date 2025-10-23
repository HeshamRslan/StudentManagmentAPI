using FastEndpoints;
using StudentManagementAPI.Services;
using StudentManagmentAPI.Models;

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public string Teacher { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateClassEndpoint : Endpoint<CreateClassRequest, object> 
{
    private readonly ClassService _classService;

    public CreateClassEndpoint(ClassService classService)
    {
        _classService = classService;
    }

    public override void Configure()
    {
        Post("/api/classes");
        AllowAnonymous();
        Validator<CreateClassRequestValidator>();
    }

    public override async Task HandleAsync(CreateClassRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Name))
        {
            await SendAsync(new { success = false, message = "Class name is required." }, 400, ct);
            return;
        }

        var newClass = new Class
        {
            Name = req.Name.Trim(),
            Teacher = req.Teacher?.Trim() ?? string.Empty,
            Description = req.Description?.Trim() ?? string.Empty
        };

        var added = _classService.Add(newClass);

        if (!added)
        {
            await SendAsync(new { success = false, message = "Failed to create class." }, 500, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Class created successfully.", data = newClass }, 201, ct);
    }
}
