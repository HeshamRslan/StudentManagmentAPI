using System.Collections.Concurrent;
using System.Collections.Generic;
using StudentManagmentAPI.Models;

namespace StudentManagementAPI.Services.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ConcurrentDictionary<int, Student> _store = new();
        private int _idCounter = 1;

        public IEnumerable<Student> GetAll() => _store.Values;

        public Student? GetById(int id) => _store.TryGetValue(id, out var s) ? s : null;

        public bool Add(Student s)
        {
            s.Id = _idCounter++;
            return _store.TryAdd(s.Id, s);
        }

        public bool Update(Student s)
        {
            if (!_store.ContainsKey(s.Id)) return false;
            _store[s.Id] = s;
            return true;
        }

        public bool Remove(int id) => _store.TryRemove(id, out _);
    }
}
