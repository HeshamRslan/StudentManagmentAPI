using FastEndpoints;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models.DTOs;

public class UpdateMarkEndpoint : Endpoint<UpdateMarkRequest, object>
{
    private readonly IMarkService _markService;

    public UpdateMarkEndpoint(IMarkService markService)
    {
        _markService = markService;
    }

    public override void Configure()
    {
        Put("/api/marks/{id}"); // Added route parameter
        AllowAnonymous();
        Validator<UpdateMarkRequestValidator>(); 
    }

    public override async Task HandleAsync(UpdateMarkRequest req, CancellationToken ct)
    {
        var id = Route<int>("id"); // Get from route
        var mark = _markService.GetById(id);

        if (mark == null)
        {
            await SendAsync(new { success = false, message = "Mark not found." }, 404, ct);
            return;
        }

        mark.ExamMark = req.ExamMark;
        mark.AssignmentMark = req.AssignmentMark;

        var updated = _markService.Update(mark);
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
