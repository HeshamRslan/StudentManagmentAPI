using FastEndpoints;
using StudentManagementAPI.Services;

public class GetAllClassesEndpoint : EndpointWithoutRequest
{
    private readonly ClassService _classService;

    public GetAllClassesEndpoint(ClassService classService)
    {
        _classService = classService;
    }

    public override void Configure()
    {
        Get("/api/classes");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var classes = _classService.GetAll();
        await SendAsync(classes, 200, ct);
    }
}
