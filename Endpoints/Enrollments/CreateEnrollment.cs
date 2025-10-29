using FastEndpoints;
using StudentManagementAPI.Services;
using StudentManagmentAPI.Models.DTOs;

namespace StudentManagementAPI.Endpoints.Enrollments
{
    public class CreateEnrollmentEndpoint : Endpoint<EnrollRequest, ApiResponse<EnrollmentResponse>>
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;

        public CreateEnrollmentEndpoint(EnrollmentService enrollmentService, StudentService studentService, ClassService classService)
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
                Data = result.Enrollment.ToResponse() }, 201, ct);
        }
    }
}
