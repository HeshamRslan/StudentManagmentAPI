using FastEndpoints;
using StudentManagementAPI.Services;

public class UpdateMarkRequest
{
    public int Id { get; set; }
    public decimal ExamMark { get; set; }
    public decimal AssignmentMark { get; set; }
}

public class UpdateMarkEndpoint : Endpoint<UpdateMarkRequest, object>
{
    private readonly MarkService _markService;

    public UpdateMarkEndpoint(MarkService markService)
    {
        _markService = markService;
    }

    public override void Configure()
    {
        Put("/api/marks");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateMarkRequest req, CancellationToken ct)
    {
        var mark = _markService.GetById(req.Id);
        if (mark == null)
        {
            await SendAsync(new { success = false, message = "Mark not found." }, 404, ct);
            return;
        }

        mark.ExamMark = req.ExamMark;
        mark.AssignmentMark = req.AssignmentMark;

        var updated = _markService.Update(req.Id, mark);
        if (!updated)
        {
            await SendAsync(new { success = false, message = "Failed to update mark." }, 500, ct);
            return;
        }

        await SendAsync(new
        {
            success = true,
            message = "Mark updated successfully.",
            data = new
            {
                mark.Id,
                mark.StudentId,
                mark.ClassId,
                mark.ExamMark,
                mark.AssignmentMark,
                mark.TotalMark
            }
        }, 200, ct);
    }
}
