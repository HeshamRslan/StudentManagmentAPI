using FastEndpoints;
using StudentManagementAPI.Mappings;
using StudentManagementAPI.Services.Interfaces;
using StudentManagmentAPI.Models.DTOs;

namespace StudentManagementAPI.Endpoints.Enrollments
{
    public class CreateEnrollmentEndpoint : Endpoint<EnrollRequest, ApiResponse<EnrollmentResponse>>
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IStudentService _studentService;
        private readonly IClassService _classService;

        public CreateEnrollmentEndpoint(IEnrollmentService enrollmentService, IStudentService studentService, IClassService classService)
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

        public override async Task HandleAsync(EnrollRequest req, CancellationToken ct)
        {
            var student = _studentService.GetById(req.StudentId);
            var cls = _classService.GetById(req.ClassId);
            if (student == null || cls == null)
            {
                await SendAsync(new ApiResponse<EnrollmentResponse>
                {
                    Success = false,
                    Message = "Invalid ids"
                }, 404, ct);
             return;
            }

            var result = await _enrollmentService.TryEnrollAsync
                (req.StudentId, req.ClassId, sid => _enrollmentService.GetByStudentId(sid).Count()
                , cid => _enrollmentService.GetByClassId(cid).Count());
            if (!result.Success) 
            {
                await SendAsync(new ApiResponse<EnrollmentResponse> 
                {
                    Success = false,
                    Message = result.Error
                }, 409, ct); return; 
            }
            await SendAsync(new ApiResponse<EnrollmentResponse> 
            {
                Success = true,
                Message = "Enrolled",
                Data = result.Created?.ToResponse() }, 201, ct);
        }
    }
}
