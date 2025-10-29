using System.Collections.Generic;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public interface IMarkRepository
    {
        IEnumerable<Mark> GetAll();
        Mark? GetById(int id);
        bool Add(Mark m);
        bool Update(Mark m);
        bool Remove(int id);
    }
}
