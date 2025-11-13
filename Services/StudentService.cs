using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Infrastructure;

namespace StudentManagementAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly ICacheService _cache;
        private readonly CacheConfiguration _cacheConfig;

        public StudentService(
            IStudentRepository repo,
            ICacheService cache,
            CacheConfiguration cacheConfig)
        {
            _repo = repo;
            _cache = cache;
            _cacheConfig = cacheConfig;
        }

        public IEnumerable<Student> GetAll()
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetAll();

            return _cache.Get<IEnumerable<Student>>(CacheKeys.AllStudents)
                ?? SetStudentsCache();
        }

        public Student? GetById(int id)
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetById(id);

            var key = CacheKeys.StudentById(id);
            return _cache.Get<Student>(key)
                ?? SetStudentByIdCache(id);
        }

        public bool Add(Student s)
        {
            var result = _repo.Add(s);
            if (result)
            {
                InvalidateStudentCache();
            }
            return result;
        }

        public bool Update(Student s)
        {
            var result = _repo.Update(s);
            if (result)
            {
                InvalidateStudentCache();
                _cache.Remove(CacheKeys.StudentById(s.Id));
            }
            return result;
        }

        public bool Remove(int id)
        {
            var result = _repo.Remove(id);
            if (result)
            {
                InvalidateStudentCache();
                _cache.Remove(CacheKeys.StudentById(id));
            }
            return result;
        }

        private IEnumerable<Student> SetStudentsCache()
        {
            var students = _repo.GetAll().ToList();
            _cache.Set(CacheKeys.AllStudents, students, _cacheConfig.StudentsCacheExpiration);
            return students;
        }

        private Student? SetStudentByIdCache(int id)
        {
            var student = _repo.GetById(id);
            if (student != null)
            {
                _cache.Set(CacheKeys.StudentById(id), student, _cacheConfig.StudentsCacheExpiration);
            }
            return student;
        }

        private void InvalidateStudentCache()
        {
            _cache.RemoveByPrefix(CacheKeys.StudentsPrefix);
        }
    }
}
