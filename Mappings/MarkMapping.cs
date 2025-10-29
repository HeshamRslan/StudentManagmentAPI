using StudentManagmentAPI.Models;
using StudentManagmentAPI.Models.DTOs;
namespace StudentManagementAPI.Mappings
{
    public static class MarkMapping
    {
        public static Mark ToModel(this CreateMarkRequest r) => new Mark
        {
            StudentId = r.StudentId,
            ClassId = r.ClassId,
            ExamMark = r.ExamMark,
            AssignmentMark = r.AssignmentMark,
            RecordedAt = DateTime.UtcNow
        };

        public static MarkResponse ToResponse(this Mark m) => new MarkResponse
        {
            Id = m.Id,
            StudentId = m.StudentId,
            ClassId = m.ClassId,
            ExamMark = m.ExamMark,
            AssignmentMark = m.AssignmentMark,
            TotalMark = m.TotalMark,
            RecordedAt = m.RecordedAt
        };
    }
}
