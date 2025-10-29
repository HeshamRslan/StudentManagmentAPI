using System.Collections.Generic;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public interface IEnrollmentRepository
    {
        IEnumerable<Enrollment> GetAll();
        Enrollment? GetById(int id);
        bool Add(Enrollment e);
        bool Remove(int id);
        Enrollment? FindByPair(int studentId, int classId);
    }
}
