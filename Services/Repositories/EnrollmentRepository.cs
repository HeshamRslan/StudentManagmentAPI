using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ConcurrentDictionary<int, Enrollment> _store = new();
        private readonly ConcurrentDictionary<string, Enrollment> _byPair = new();
        private int _idCounter = 1;

        public IEnumerable<Enrollment> GetAll() => _store.Values;

        public Enrollment? GetById(int id) => _store.TryGetValue(id, out var e) ? e : null;

        public bool Add(Enrollment e)
        {
            var key = $"{e.StudentId}-{e.ClassId}";
            if (!_byPair.TryAdd(key, e))
                return false;
            e.Id = _idCounter++;
            var added = _store.TryAdd(e.Id, e);
            if (!added) _byPair.TryRemove(key, out _);
            return added;
        }

        public bool Remove(int id)
        {
            if (!_store.TryRemove(id, out var e)) return false;
            var key = $"{e.StudentId}-{e.ClassId}";
            _byPair.TryRemove(key, out _);
            return true;
        }

        public Enrollment? FindByPair(int studentId, int classId)
        {
            var key = $"{studentId}-{classId}";
            return _byPair.TryGetValue(key, out var e) ? e : null;
        }
    }
}
