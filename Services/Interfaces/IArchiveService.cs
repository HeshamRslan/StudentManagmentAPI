using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Interfaces
{
    public interface IArchiveService
    {
        IEnumerable<Class> GetAll();
        bool Archive(Class cls);
    }
}