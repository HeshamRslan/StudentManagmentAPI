using FastEndpoints;
using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Interfaces;

public class CreateMarkRequest
{
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public decimal ExamMark { get; set; }
    public decimal AssignmentMark { get; set; }
}

public class CreateMarkEndpoint : Endpoint<CreateMarkRequest, object>
{
    private readonly IMarkService _markService;
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;

    public CreateMarkEndpoint(IMarkService markService, IStudentService studentService, IClassService classService)
    {
        _markService = markService;
        _studentService = studentService;
        _classService = classService;
    }

    public override void Configure()
    {
        Post("/api/marks");
        AllowAnonymous();
        Validator<CreateMarkRequestValidator>();

    }

    public override async Task HandleAsync(CreateMarkRequest req, CancellationToken ct)
    {
        var student = _studentService.GetById(req.StudentId);
        var cls = _classService.GetById(req.ClassId);

        if (student == null || cls == null)
        {
            await SendAsync(new { success = false, message = "Invalid StudentId or ClassId." }, 400, ct);
            return;
        }

        var mark = new Mark
        {
            StudentId = req.StudentId,
            ClassId = req.ClassId,
            ExamMark = req.ExamMark,
            AssignmentMark = req.AssignmentMark
        };

        var added = _markService.Add(mark);
        if (!added)
        {
            await SendAsync(new { success = false, message = "Failed to add mark." }, 500, ct);
            return;
        }

        await SendAsync(new
        {
            success = true,
            message = $"Marks added successfully for student '{student.FirstName}' in class '{cls.Name}'.",
            data = mark
        }, 201, ct);
    }
}
