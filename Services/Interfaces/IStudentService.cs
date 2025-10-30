using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IStudentService
    {
        IEnumerable<Student> GetAll();
        Student? GetById(int id);
        bool Add(Student student);
        bool Update(Student student);
        bool Remove(int id);
    }
}
