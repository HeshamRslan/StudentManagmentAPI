namespace StudentManagementAPI.Services.Repositories
{
    using StudentManagmentAPI.Models;
    public interface IClassRepository
    {
        IEnumerable<Class> GetAll();
        Class? GetById(int id);
        bool Add(Class cls);
        bool Remove(int id);
        bool Update(Class cls);
    }
}
