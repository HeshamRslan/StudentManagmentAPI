using System.Collections.Concurrent;
using System.Collections.Generic;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public class MarkRepository : IMarkRepository
    {
        private readonly ConcurrentDictionary<int, Mark> _store = new();
        private int _idCounter = 1;

        public IEnumerable<Mark> GetAll() => _store.Values;

        public Mark? GetById(int id) => _store.TryGetValue(id, out var m) ? m : null;

        public bool Add(Mark m)
        {
            m.Id = _idCounter++;
            return _store.TryAdd(m.Id, m);
        }

        public bool Update(Mark m)
        {
            if (!_store.ContainsKey(m.Id)) return false;
            _store[m.Id] = m;
            return true;
        }

        public bool Remove(int id) => _store.TryRemove(id, out _);
    }
}
