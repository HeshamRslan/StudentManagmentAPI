using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Infrastructure;

namespace StudentManagementAPI.Services
{
    public class MarkService : IMarkService
    {
        private readonly IMarkRepository _repo;
        private readonly ICacheService _cache;
        private readonly CacheConfiguration _cacheConfig;

        public MarkService(
            IMarkRepository repo,
            ICacheService cache,
            CacheConfiguration cacheConfig)
        {
            _repo = repo;
            _cache = cache;
            _cacheConfig = cacheConfig;
        }

        public IEnumerable<Mark> GetAll()
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetAll();

            return _cache.Get<IEnumerable<Mark>>(CacheKeys.AllMarks)
                ?? SetAllMarksCache();
        }

        public Mark? GetById(int id)
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetById(id);

            var key = CacheKeys.MarkById(id);
            return _cache.Get<Mark>(key)
                ?? SetMarkByIdCache(id);
        }

        public bool Add(Mark m)
        {
            var result = _repo.Add(m);
            if (result)
            {
                InvalidateMarkCache();
                // Also invalidate related caches
                _cache.Remove(CacheKeys.MarksByStudent(m.StudentId));
                _cache.Remove(CacheKeys.MarksByClass(m.ClassId));
                _cache.Remove(CacheKeys.ClassAverage(m.ClassId));
                _cache.Remove(CacheKeys.StudentReport(m.StudentId));
            }
            return result;
        }

        public bool Update(Mark m)
        {
            var result = _repo.Update(m);
            if (result)
            {
                InvalidateMarkCache();
                _cache.Remove(CacheKeys.MarkById(m.Id));
                _cache.Remove(CacheKeys.MarksByStudent(m.StudentId));
                _cache.Remove(CacheKeys.MarksByClass(m.ClassId));
                _cache.Remove(CacheKeys.ClassAverage(m.ClassId));
                _cache.Remove(CacheKeys.StudentReport(m.StudentId));
            }
            return result;
        }

        public bool Remove(int id)
        {
            var mark = _repo.GetById(id);
            var result = _repo.Remove(id);
            if (result && mark != null)
            {
                InvalidateMarkCache();
                _cache.Remove(CacheKeys.MarkById(id));
                _cache.Remove(CacheKeys.MarksByStudent(mark.StudentId));
                _cache.Remove(CacheKeys.MarksByClass(mark.ClassId));
                _cache.Remove(CacheKeys.ClassAverage(mark.ClassId));
                _cache.Remove(CacheKeys.StudentReport(mark.StudentId));
            }
            return result;
        }

        public IEnumerable<Mark> GetByStudent(int studentId)
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetAll().Where(x => x.StudentId == studentId);

            var key = CacheKeys.MarksByStudent(studentId);
            return _cache.Get<IEnumerable<Mark>>(key)
                ?? SetMarksByStudentCache(studentId);
        }

        public IEnumerable<Mark> GetByClass(int classId)
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetAll().Where(x => x.ClassId == classId);

            var key = CacheKeys.MarksByClass(classId);
            return _cache.Get<IEnumerable<Mark>>(key)
                ?? SetMarksByClassCache(classId);
        }

        private IEnumerable<Mark> SetAllMarksCache()
        {
            var marks = _repo.GetAll().ToList();
            _cache.Set(CacheKeys.AllMarks, marks, _cacheConfig.MarksCacheExpiration);
            return marks;
        }

        private Mark? SetMarkByIdCache(int id)
        {
            var mark = _repo.GetById(id);
            if (mark != null)
            {
                _cache.Set(CacheKeys.MarkById(id), mark, _cacheConfig.MarksCacheExpiration);
            }
            return mark;
        }

        private IEnumerable<Mark> SetMarksByStudentCache(int studentId)
        {
            var marks = _repo.GetAll().Where(x => x.StudentId == studentId).ToList();
            _cache.Set(CacheKeys.MarksByStudent(studentId), marks, _cacheConfig.MarksCacheExpiration);
            return marks;
        }

        private IEnumerable<Mark> SetMarksByClassCache(int classId)
        {
            var marks = _repo.GetAll().Where(x => x.ClassId == classId).ToList();
            _cache.Set(CacheKeys.MarksByClass(classId), marks, _cacheConfig.MarksCacheExpiration);
            return marks;
        }

        private void InvalidateMarkCache()
        {
            _cache.RemoveByPrefix(CacheKeys.MarksPrefix);
        }
    }
}