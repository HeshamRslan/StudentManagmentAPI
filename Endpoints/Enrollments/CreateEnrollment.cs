using FastEndpoints;
using StudentManagementAPI.Services;

public class CreateEnrollmentRequest
{
    public int StudentId { get; set; }
    public int ClassId { get; set; }
}

public class CreateEnrollmentEndpoint : Endpoint<CreateEnrollmentRequest, object>
{
    private readonly EnrollmentService _enrollmentService;
    private readonly StudentService _studentService;
    private readonly ClassService _classService;

    public CreateEnrollmentEndpoint(EnrollmentService enrollmentService, StudentService studentService, ClassService classService)
    {
        _enrollmentService = enrollmentService;
        _studentService = studentService;
        _classService = classService;
    }

    public override void Configure()
    {
        Post("/api/enrollments");
        AllowAnonymous();
        Validator<CreateEnrollmentRequestValidator>();
    }

    public override async Task HandleAsync(CreateEnrollmentRequest req, CancellationToken ct)
    {
        // basic existence checks
        var student = _studentService.GetById(req.StudentId);
        if (student == null)
        {
            await SendAsync(new { success = false, message = "Student not found." }, 404, ct);
            return;
        }
        var cls = _classService.GetById(req.ClassId);
        if (cls == null)
        {
            await SendAsync(new { success = false, message = "Class not found." }, 404, ct);
            return;
        }

        // Call TryEnrollAsync, pass callbacks to get counts
        var result = await _enrollmentService.TryEnrollAsync(
            req.StudentId,
            req.ClassId,
            studentId => _enrollmentService.GetByStudentId(studentId).Count(),   // student count provider
            classId => _enrollmentService.GetByClassId(classId).Count(),         // class count provider
            maxClassesPerStudent: 5,
            maxStudentsPerClass: 30);

        if (!result.Success)
        {
            await SendAsync(new { success = false, message = result.Error }, 409, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Enrolled successfully.", data = result.Enrollment }, 201, ct);
    }
}
