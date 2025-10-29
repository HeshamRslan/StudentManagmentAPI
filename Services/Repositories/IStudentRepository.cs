using System.Collections.Generic;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student? GetById(int id);
        bool Add(Student s);
        bool Update(Student s);
        bool Remove(int id);
    }
}
