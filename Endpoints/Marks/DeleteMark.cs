using FastEndpoints;
using StudentManagementAPI.Services.Interfaces;

public class DeleteMarkEndpoint : EndpointWithoutRequest<object>
{
    private readonly IMarkService _markService;

    public DeleteMarkEndpoint(IMarkService markService)
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
