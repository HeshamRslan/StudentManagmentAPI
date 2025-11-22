using FastEndpoints;
using StudentManagmentAPI.Models.DTOs;
using StudentManagementAPI.Services.Interfaces;

namespace StudentManagementAPI.Endpoints.Classes
{
    public class DeleteClassEndpoint : EndpointWithoutRequest<ApiResponse<object>>
    {
        private readonly IClassService _classService;

        public DeleteClassEndpoint(IClassService classService)
        {
            _classService = classService;
        }

        public override void Configure()
        {
            Delete("/api/classes/{id:int}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<int>("id");

            var (success, error) = _classService.Delete(id);

            if (!success)
            {
                await SendAsync(new ApiResponse<object>
                {
                    Success = false,
                    Message = error ?? "Failed to delete class."
                }, 400, ct);
                return;
            }

            await SendAsync(new ApiResponse<object>
            {
                Success = true,
                Message = $"Class with ID {id} deleted successfully."
            }, 200, ct);
        }
    }
}
