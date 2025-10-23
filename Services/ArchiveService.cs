using StudentManagmentAPI.Models;
using System.Collections.Concurrent;

namespace StudentManagementAPI.Services
{
    public class ArchiveService
    {
        private readonly ConcurrentDictionary<int, Class> _archived = new();
        private int _idCounter = 1;

        public IEnumerable<Class> GetAll() => _archived.Values;

        public bool Archive(Class cls)
        {
            // keep original id
            return _archived.TryAdd(cls.Id, cls);
        }
    }
}
