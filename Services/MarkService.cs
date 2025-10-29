using System.Collections.Generic;
using System.Linq;
using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Repositories;

namespace StudentManagementAPI.Services
{
    public class MarkService
    {
        private readonly IMarkRepository _repo;

        public MarkService(IMarkRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Mark> GetAll() => _repo.GetAll();

        public Mark? GetById(int id) => _repo.GetById(id);

        public bool Add(Mark m) => _repo.Add(m);

        public bool Update(Mark m) => _repo.Update(m);

        public bool Remove(int id) => _repo.Remove(id);

        public IEnumerable<Mark> GetByStudent(int studentId) => _repo.GetAll().Where(x => x.StudentId == studentId);

        public IEnumerable<Mark> GetByClass(int classId) => _repo.GetAll().Where(x => x.ClassId == classId);
    }
}
