using System.Collections.Generic;
using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Repositories;

namespace StudentManagementAPI.Services
{
    public class StudentService
    {
        private readonly IStudentRepository _repo;

        public StudentService(IStudentRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Student> GetAll() => _repo.GetAll();

        public Student? GetById(int id) => _repo.GetById(id);

        public bool Add(Student s) => _repo.Add(s);

        public bool Update(Student s) => _repo.Update(s);

        public bool Remove(int id) => _repo.Remove(id);
    }
}
