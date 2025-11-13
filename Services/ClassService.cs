using Microsoft.Extensions.Logging;
using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Infrastructure;

namespace StudentManagementAPI.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repo;
        private readonly ILogger<ClassService> _logger;
        private readonly ICacheService _cache;
        private readonly CacheConfiguration _cacheConfig;
        private readonly IEnrollmentService? _enrollmentService;

        public ClassService(
            IClassRepository repo,
            ILogger<ClassService> logger,
            ICacheService cache,
            CacheConfiguration cacheConfig,
            IEnrollmentService? enrollmentService = null)
        {
            _repo = repo;
            _logger = logger;
            _cache = cache;
            _cacheConfig = cacheConfig;
            _enrollmentService = enrollmentService;
        }

        public IEnumerable<Class> GetAll(string? search = null)
        {
            if (!_cacheConfig.EnableCaching)
                return GetClassesFromRepo(search);

            if (string.IsNullOrWhiteSpace(search))
            {
                return _cache.Get<IEnumerable<Class>>(CacheKeys.AllClasses)
                    ?? SetAllClassesCache();
            }

            var key = CacheKeys.ClassSearch(search);
            return _cache.Get<IEnumerable<Class>>(key)
                ?? SetClassSearchCache(search);
        }

        public Class? GetById(int id)
        {
            if (!_cacheConfig.EnableCaching)
                return _repo.GetById(id);

            var key = CacheKeys.ClassById(id);
            return _cache.Get<Class>(key)
                ?? SetClassByIdCache(id);
        }

        public (bool Success, string? Error, Class? Created) Create(Class cls)
        {
            if (string.IsNullOrWhiteSpace(cls.Name))
                return (false, "Class name is required.", null);

            if (_repo.GetAll().Any(c => c.Name.Equals(cls.Name, StringComparison.OrdinalIgnoreCase)))
                return (false, "Class name already exists.", null);

            var ok = _repo.Add(cls);
            if (!ok) return (false, "Failed to add class.", null);

            _logger.LogInformation("Class '{Name}' created (Id: {Id})", cls.Name, cls.Id);
            InvalidateClassCache();

            return (true, null, cls);
        }

        public (bool Success, string? Error) Delete(int id)
        {
            if (_enrollmentService != null)
            {
                var count = _enrollmentService.GetByClassId(id).Count();
                if (count > 0) return (false, "Cannot delete class with enrolled students.");
            }

            var ok = _repo.Remove(id);
            if (!ok) return (false, "Failed to remove class.");

            _logger.LogInformation("Class {Id} removed", id);
            InvalidateClassCache();
            _cache.Remove(CacheKeys.ClassById(id));

            return (true, null);
        }

        public (bool Success, string? Error) Update(Class cls)
        {
            if (!_repo.GetAll().Any(c => c.Id == cls.Id))
                return (false, "Class not found");

            var ok = _repo.Update(cls);
            if (!ok) return (false, "Failed to update class");

            InvalidateClassCache();
            _cache.Remove(CacheKeys.ClassById(cls.Id));

            return (true, null);
        }

        public int GetEnrolledCount(int classId)
        {
            if (_enrollmentService == null) return 0;

            if (!_cacheConfig.EnableCaching)
                return _enrollmentService.GetByClassId(classId).Count();

            var key = CacheKeys.ClassEnrollmentCount(classId);
            var cached = _cache.Get<int?>(key);
            if (cached.HasValue) return cached.Value;

            var count = _enrollmentService.GetByClassId(classId).Count();
            _cache.Set(key, count, _cacheConfig.EnrollmentsCacheExpiration);
            return count;
        }

        private IEnumerable<Class> GetClassesFromRepo(string? search)
        {
            var data = _repo.GetAll();
            if (string.IsNullOrWhiteSpace(search)) return data;
            return data.Where(c =>
                c.Name!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Teacher!.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<Class> SetAllClassesCache()
        {
            var classes = _repo.GetAll().ToList();
            _cache.Set(CacheKeys.AllClasses, classes, _cacheConfig.ClassesCacheExpiration);
            return classes;
        }

        private IEnumerable<Class> SetClassSearchCache(string search)
        {
            var classes = GetClassesFromRepo(search).ToList();
            _cache.Set(CacheKeys.ClassSearch(search), classes, _cacheConfig.ClassesCacheExpiration);
            return classes;
        }

        private Class? SetClassByIdCache(int id)
        {
            var cls = _repo.GetById(id);
            if (cls != null)
            {
                _cache.Set(CacheKeys.ClassById(id), cls, _cacheConfig.ClassesCacheExpiration);
            }
            return cls;
        }

        private void InvalidateClassCache()
        {
            _cache.RemoveByPrefix(CacheKeys.ClassesPrefix);
        }
    }
}