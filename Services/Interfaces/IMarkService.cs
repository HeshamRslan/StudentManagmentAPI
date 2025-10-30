using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IMarkService
    {
        IEnumerable<Mark> GetAll();
        Mark? GetById(int id);
        bool Add(Mark mark);
        bool Update(Mark mark);
        bool Remove(int id);
        IEnumerable<Mark> GetByStudent(int studentId);
        IEnumerable<Mark> GetByClass(int classId);
    }
}