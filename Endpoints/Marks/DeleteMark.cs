using FastEndpoints;
using StudentManagementAPI.Services;

public class DeleteMarkEndpoint : EndpointWithoutRequest<object>
{
    private readonly MarkService _markService;

    public DeleteMarkEndpoint(MarkService markService)
    {
        _markService = markService;
    }

    public override void Configure()
    {
        Delete("/api/marks/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        var deleted = _markService.Remove(id);

        if (!deleted)
        {
            await SendAsync(new { success = false, message = "Mark not found." }, 404, ct);
            return;
        }

        await SendAsync(new { success = true, message = "Mark deleted successfully." }, 200, ct);
    }
}
