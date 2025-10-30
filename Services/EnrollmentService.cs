using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudentManagmentAPI.Models;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Interfaces;

namespace StudentManagementAPI.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        // simple locks per pair to prevent race conditions
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public EnrollmentService(IEnrollmentRepository repo)
        {
            _repo = repo;
        }

        private SemaphoreSlim GetLock(int studentId, int classId) =>
            _locks.GetOrAdd($"{studentId}-{classId}", _ => new SemaphoreSlim(1,1));

        public IEnumerable<Enrollment> GetAll() => _repo.GetAll();
        public IEnumerable<Enrollment> GetByStudentId(int studentId) => _repo.GetAll().Where(e => e.StudentId == studentId);
        public IEnumerable<Enrollment> GetByClassId(int classId) => _repo.GetAll().Where(e => e.ClassId == classId);

        public async Task<(bool Success, string? Error, Enrollment? Created)> TryEnrollAsync(int studentId, int classId, Func<int,int> getStudentCount, Func<int,int> getClassCount, int maxPerStudent = 5, int maxPerClass = 30)
        {
            var key = $"{studentId}-{classId}";
            var sem = GetLock(studentId, classId);
            await sem.WaitAsync();
            try
            {
                // duplicate check
                if (_repo.FindByPair(studentId, classId) != null)
                    return (false, "Student is already enrolled in this class.", null);

                // checks
                if (getStudentCount(studentId) >= maxPerStudent)
                    return (false, $"Student cannot enroll in more than {maxPerStudent} classes.", null);

                if (getClassCount(classId) >= maxPerClass)
                    return (false, $"Class is full (max {maxPerClass} students).", null);

                var e = new Enrollment { StudentId = studentId, ClassId = classId, EnrollmentDate = DateTime.UtcNow };
                var ok = _repo.Add(e);
                if (!ok) return (false, "Failed to add enrollment.", null);
                return (true, null, e);
            }
            finally
            {
                sem.Release();
            }
        }

        public bool UnenrollByPair(int studentId, int classId)
        {
            var existing = _repo.FindByPair(studentId, classId);
            if (existing == null) return false;
            return _repo.Remove(existing.Id);
        }
    }
}
