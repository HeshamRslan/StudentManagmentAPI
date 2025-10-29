using FastEndpoints;
using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;

public class GetAllClassesEndpoint : EndpointWithoutRequest
{
    private readonly IClassService _classService;

    public GetAllClassesEndpoint(IClassService classService)
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
