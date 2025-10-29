using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IClassService
    {
        IEnumerable<Class> GetAll(string? search = null);
        Class? GetById(int id);
        (bool Success, string? Error, Class? Created) Create(Class cls);
        (bool Success, string? Error) Delete(int id);
        (bool Success, string? Error) Update(Class cls);
        int GetEnrolledCount(int classId); // to be implemented using EnrollmentService later
    }
}
