using StudentManagmentAPI.Models;
using System.Collections.Concurrent;

namespace StudentManagementAPI.Services
{
    public class ClassService
    {
        private readonly ConcurrentDictionary<int, Class> _classes = new();
        private int _idCounter = 1;

        public IEnumerable<Class> GetAll() => _classes.Values;

        public Class? GetById(int id)
            => _classes.TryGetValue(id, out var cls) ? cls : null;

        public bool Add(Class cls)
        {
            cls.Id = _idCounter++;
            return _classes.TryAdd(cls.Id, cls);
        }

        public bool Remove(int id)
            => _classes.TryRemove(id, out _);
    }
}
