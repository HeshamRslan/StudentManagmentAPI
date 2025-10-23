using FastEndpoints;
using StudentManagementAPI.Services;

public class GetStudentReportEndpoint : EndpointWithoutRequest<object>
{
    private readonly StudentService _studentService;
    private readonly ClassService _classService;
    private readonly MarkService _markService;
    private readonly EnrollmentService _enrollmentService;

    public GetStudentReportEndpoint(StudentService studentService, ClassService classService, MarkService markService, EnrollmentService enrollmentService)
    {
        _studentService = studentService;
        _classService = classService;
        _markService = markService;
        _enrollmentService = enrollmentService;
    }

    public override void Configure()
    {
        Get("/api/students/{studentId}/report");
        AllowAnonymous();
    }

    private string GradeFromAvg(decimal avg) =>
        avg >= 90 ? "A" :
        avg >= 80 ? "B" :
        avg >= 70 ? "C" :
        avg >= 60 ? "D" : "F";

    private string Ordinal(int n)
    {
        if (n % 100 >= 11 && n % 100 <= 13) return n + "th";
        return (n % 10) switch { 1 => n + "st", 2 => n + "nd", 3 => n + "rd", _ => n + "th" };
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var studentId = Route<int>("studentId");
        var student = _studentService.GetById(studentId);
        if (student == null)
        {
            await SendAsync(new { success = false, message = "Student not found." }, 404, ct);
            return;
        }

        // all marks for student
        var marks = _markService.GetAll().Where(m => m.StudentId == studentId).ToList();
        var enrolledClassIds = _enrollmentService.GetByStudentId(studentId).Select(e => e.ClassId).Distinct().ToList();

        var classes = new List<object>();
        foreach (var classId in enrolledClassIds)
        {
            var cls = _classService.GetById(classId);
            if (cls == null) continue;

            // all marks in this class for all students
            var classMarksAll = _markService.GetAll().Where(m => m.ClassId == classId).ToList();
            // marks for this student in this class
            var studentMarks = classMarksAll.Where(m => m.StudentId == studentId).OrderBy(m => m.RecordedAt).ToList();

            decimal studentAvgInClass = studentMarks.Any() ? studentMarks.Average(m => m.TotalMark) : 0m;

            // ranking
            var rankingList = classMarksAll
                .GroupBy(m => m.StudentId)
                .Select(g => new { StudentId = g.Key, Avg = g.Average(m => m.TotalMark) })
                .OrderByDescending(x => x.Avg)
                .ToList();

            var rankIndex = rankingList.FindIndex(x => x.StudentId == studentId);
            int rank = rankIndex >= 0 ? rankIndex + 1 : rankingList.Count + 1;
            int totalStudentsInClass = rankingList.Count;

            // grade
            var grade = GradeFromAvg(studentAvgInClass);

            // trend: simple compare first half vs last half of student's marks in this class (if enough)
            string trend = "stable";
            if (studentMarks.Count >= 2)
            {
                var half = studentMarks.Count / 2;
                var firstAvg = studentMarks.Take(half).Any() ? studentMarks.Take(half).Average(m => m.TotalMark) : studentMarks.First().TotalMark;
                var lastAvg = studentMarks.Skip(half).Any() ? studentMarks.Skip(half).Average(m => m.TotalMark) : studentMarks.Last().TotalMark;
                if (lastAvg > firstAvg + 1) trend = "improving";
                else if (lastAvg < firstAvg - 1) trend = "declining";
                else trend = "stable";
            }

            classes.Add(new
            {
                id = cls.Id,
                name = cls.Name,
                teacher = cls.Teacher,
                studentAverage = Math.Round(studentAvgInClass, 2),
                grade,
                rank = $"{Ordinal(rank)} out of {totalStudentsInClass}",
                trend,
                marks = studentMarks.Select(m => new { m.ExamMark, m.AssignmentMark, m.TotalMark, m.RecordedAt }).ToList()
            });
        }

        var overallAverage = marks.Any() ? Math.Round(marks.Average(m => m.TotalMark), 2) : 0m;

        await SendAsync(new
        {
            success = true,
            student = new { student.Id, student.FirstName, student.LastName },
            classes,
            overallAverage
        }, 200, ct);
    }
}
