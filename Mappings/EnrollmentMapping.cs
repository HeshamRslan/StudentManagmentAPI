using StudentManagmentAPI.Models;
using StudentManagmentAPI.Models.DTOs;
namespace StudentManagementAPI.Mappings
{
    public static class EnrollmentMapping
    {
        public static EnrollmentResponse ToResponse(this Enrollment e) => new EnrollmentResponse
        {
            Id = e.Id,
            StudentId = e.StudentId,
            ClassId = e.ClassId,
            EnrolledAt = e.EnrollmentDate
        };
    }
}
