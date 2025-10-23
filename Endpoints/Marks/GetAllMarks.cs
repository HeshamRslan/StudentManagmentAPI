using FastEndpoints;
using StudentManagementAPI.Services;

public class GetAllMarksRequest
{
    public int? StudentId { get; set; }
    public int? ClassId { get; set; }
}

public class GetAllMarksEndpoint : Endpoint<GetAllMarksRequest, object>
{
    private readonly MarkService _markService;

    public GetAllMarksEndpoint(MarkService markService)
    {
        _markService = markService;
    }

    public override void Configure()
    {
        Get("/api/marks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllMarksRequest req, CancellationToken ct)
    {
        var all = _markService.GetAll();

        if (req.StudentId.HasValue)
            all = all.Where(m => m.StudentId == req.StudentId.Value);

        if (req.ClassId.HasValue)
            all = all.Where(m => m.ClassId == req.ClassId.Value);

        await SendAsync(new
        {
            success = true,
            total = all.Count(),
            data = all.Select(m => new
            {
                m.Id,
                m.StudentId,
                m.ClassId,
                m.ExamMark,
                m.AssignmentMark,
                m.TotalMark
            })
        }, 200, ct);
    }
}
