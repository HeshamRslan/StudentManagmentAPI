using FastEndpoints;
using StudentManagementAPI.Services;

public class UpdateStudentRequest
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class UpdateStudentEndpoint : Endpoint<UpdateStudentRequest>
{
    private readonly StudentService _studentService;

    public UpdateStudentEndpoint(StudentService studentService)
    {
        _studentService = studentService;
    }

    public override void Configure()
    {
        Put("/api/students");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateStudentRequest req, CancellationToken ct)
    {
        var existing = _studentService.GetById(req.Id);

        if (existing == null)
        {
            await SendAsync(new { success = false, message = "Student not found." }, 404, ct);
            return;
        }

        existing.FirstName = req.FirstName.Trim();
        existing.LastName = req.LastName.Trim();
        existing.Age = req.Age;

        var updated = _studentService.Update(existing);

        if (!updated)
        {
            await SendAsync(new { success = false, message = "Failed to update student." }, 500, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Student updated successfully.", student = existing }, 200, ct);
    }
}
