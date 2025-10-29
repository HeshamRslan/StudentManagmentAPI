using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Interfaces;

namespace StudentManagementAPI.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repo;
        private readonly ILogger<ClassService> _logger;
        private readonly IMemoryCache _cache;
        // Dependency on EnrollmentService is optional; inject if available to check counts.
        private readonly EnrollmentService? _enrollmentService;

        public ClassService(IClassRepository repo, ILogger<ClassService> logger, IMemoryCache cache, EnrollmentService? enrollmentService = null)
        {
            _repo = repo;
            _logger = logger;
            _cache = cache;
            _enrollmentService = enrollmentService;
        }

        public IEnumerable<Class> GetAll(string? search = null)
        {
            var data = _repo.GetAll();
            if (string.IsNullOrWhiteSpace(search)) return data;
            return data.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                   c.Teacher.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        public Class? GetById(int id) => _repo.GetById(id);

        public (bool Success, string? Error, Class? Created) Create(Class cls)
        {
            // business validations
            if (string.IsNullOrWhiteSpace(cls.Name))
                return (false, "Class name is required.", null);

            // prevent duplicate by name
            if (_repo.GetAll().Any(c => c.Name.Equals(cls.Name, StringComparison.OrdinalIgnoreCase)))
                return (false, "Class name already exists.", null);

            var ok = _repo.Add(cls);
            if (!ok) return (false, "Failed to add class.", null);

            _logger.LogInformation("Class '{Name}' created (Id: {Id})", cls.Name, cls.Id);
            // clear cache if used for class lists
            _cache.Remove("classes_all");
            return (true, null, cls);
        }

        public (bool Success, string? Error) Delete(int id)
        {
            // check enrollments via enrollmentService if injected
            if (_enrollmentService != null)
            {
                var count = _enrollmentService.GetByClassId(id).Count();
                if (count > 0) return (false, "Cannot delete class with enrolled students.");
            }

            var ok = _repo.Remove(id);
            if (!ok) return (false, "Failed to remove class.");
            _logger.LogInformation("Class {Id} removed", id);
            _cache.Remove("classes_all");
            return (true, null);
        }

        public (bool Success, string? Error) Update(Class cls)
        {
            if (!_repo.GetAll().Any(c => c.Id == cls.Id))
                return (false, "Class not found");

            var ok = _repo.Update(cls);
            if (!ok) return (false, "Failed to update class");
            _cache.Remove("classes_all");
            return (true, null);
        }

        public int GetEnrolledCount(int classId)
        {
            if (_enrollmentService == null) return 0;
            return _enrollmentService.GetByClassId(classId).Count();
        }
    }
}
