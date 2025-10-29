using System.Collections.Concurrent;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly ConcurrentDictionary<int, Class> _store = new();
        private int _idCounter = 1;

        public IEnumerable<Class> GetAll() => _store.Values;

        public Class? GetById(int id) => _store.TryGetValue(id, out var c) ? c : null;

        public bool Add(Class cls)
        {
            cls.Id = _idCounter++;
            cls.CreatedAt = cls.CreatedAt == default ? DateTime.UtcNow : cls.CreatedAt;
            return _store.TryAdd(cls.Id, cls);
        }

        public bool Remove(int id) => _store.TryRemove(id, out _);

        public bool Update(Class cls)
        {
            if (!_store.ContainsKey(cls.Id)) return false;
            _store[cls.Id] = cls;
            return true;
        }
    }
}
