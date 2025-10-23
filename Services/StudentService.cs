using StudentManagmentAPI.Models;
using System.Collections.Concurrent;

namespace StudentManagementAPI.Services
{
    public class StudentService
    {
        private readonly ConcurrentDictionary<int, Student> _students = new();
        private int _idCounter = 1;

        public IEnumerable<Student> GetAll() => _students.Values;

        public Student? GetById(int id)
            => _students.TryGetValue(id, out var s) ? s : null;

        public bool Add(Student student)
        {
            student.Id = _idCounter++;
            return _students.TryAdd(student.Id, student);
        }

        public bool Update(Student updatedStudent)
        {
            if (!_students.ContainsKey(updatedStudent.Id))
                return false; 

            _students[updatedStudent.Id] = updatedStudent;
            return true;
        }

        public bool Remove(int id)
            => _students.TryRemove(id, out _);
    }
}
