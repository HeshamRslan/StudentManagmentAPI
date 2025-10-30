using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IEnrollmentService
    {
        IEnumerable<Enrollment> GetAll();
        IEnumerable<Enrollment> GetByStudentId(int studentId);
        IEnumerable<Enrollment> GetByClassId(int classId);
        Task<(bool Success, string? Error, Enrollment? Created)> TryEnrollAsync(
            int studentId,
            int classId,
            Func<int, int> getStudentCount,
            Func<int, int> getClassCount,
            int maxPerStudent = 5,
            int maxPerClass = 30);
        bool UnenrollByPair(int studentId, int classId);
    }
}