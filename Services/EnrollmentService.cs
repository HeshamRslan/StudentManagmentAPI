using StudentManagmentAPI.Models;
using System.Collections.Concurrent;

namespace StudentManagementAPI.Services
{
    public class EnrollmentService
    {
        // key = enrollmentId -> Enrollment (for listing)
        private readonly ConcurrentDictionary<int, Enrollment> _enrollments = new();
        private int _idCounter = 1;

        // key pair for uniqueness check: "studentId-classId"
        private readonly ConcurrentDictionary<string, Enrollment> _byPair = new();

        // Locks per entity to coordinate counts and additions
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _studentLocks = new();
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _classLocks = new();

        private SemaphoreSlim GetStudentLock(int studentId) =>
            _studentLocks.GetOrAdd(studentId, _ => new SemaphoreSlim(1, 1));
        private SemaphoreSlim GetClassLock(int classId) =>
            _classLocks.GetOrAdd(classId, _ => new SemaphoreSlim(1, 1));

        public IEnumerable<Enrollment> GetAll() => _enrollments.Values;

        public IEnumerable<Enrollment> GetByStudentId(int studentId) =>
            _enrollments.Values.Where(e => e.StudentId == studentId);

        public IEnumerable<Enrollment> GetByClassId(int classId) =>
            _enrollments.Values.Where(e => e.ClassId == classId);

        // TryEnrollAsync: atomic, thread-safe, enforces rules (limits) and prevents duplicates
        public async Task<(bool Success, string? Error, Enrollment? Enrollment)> TryEnrollAsync(
            int studentId,
            int classId,
            Func<int, int> getStudentEnrollmentCount,  // callback to get count
            Func<int, int> getClassEnrollmentCount,    // callback to get count
            int maxClassesPerStudent = 5,
            int maxStudentsPerClass = 30)
        {
            // Compose key to check duplicates
            var key = $"{studentId}-{classId}";

            // quick check using _byPair
            if (_byPair.ContainsKey(key))
                return (false, "Student is already enrolled in this class.", null);

            // Acquire locks in consistent order to avoid deadlocks (lower id first)
            var firstLock = studentId <= classId ? (GetStudentLock(studentId), GetClassLock(classId)) : (GetClassLock(classId), GetStudentLock(studentId));
            await firstLock.Item1.WaitAsync();
            await firstLock.Item2.WaitAsync();

            try
            {
                // re-check duplicate after acquiring locks
                if (_byPair.ContainsKey(key))
                    return (false, "Student is already enrolled in this class.", null);

                // check student limit
                var studentCount = getStudentEnrollmentCount(studentId);
                if (studentCount >= maxClassesPerStudent)
                    return (false, $"Student cannot enroll in more than {maxClassesPerStudent} classes.", null);

                // check class capacity
                var classCount = getClassEnrollmentCount(classId);
                if (classCount >= maxStudentsPerClass)
                    return (false, $"Class is full (max {maxStudentsPerClass} students).", null);

                // add enrollment
                var enrollment = new Enrollment
                {
                    Id = _idCounter++,
                    StudentId = studentId,
                    ClassId = classId,
                    EnrollmentDate = DateTime.UtcNow
                };

                if (!_enrollments.TryAdd(enrollment.Id, enrollment))
                    return (false, "Failed to add enrollment (internal).", null);

                if (!_byPair.TryAdd(key, enrollment))
                {
                    // rollback if pair add failed
                    _enrollments.TryRemove(enrollment.Id, out _);
                    return (false, "Enrollment already exists (race).", null);
                }

                return (true, null, enrollment);
            }
            finally
            {
                firstLock.Item2.Release();
                firstLock.Item1.Release();
            }
        }

        // Remove enrollment by id
        public bool Remove(int id)
        {
            if (!_enrollments.TryRemove(id, out var e))
                return false;
            var key = $"{e.StudentId}-{e.ClassId}";
            _byPair.TryRemove(key, out _);
            return true;
        }

        // Optional helper: remove by pair (studentId,classId) if you want
        public bool RemoveByPair(int studentId, int classId)
        {
            var key = $"{studentId}-{classId}";
            if (!_byPair.TryRemove(key, out var e))
                return false;
            _enrollments.TryRemove(e.Id, out _);
            return true;
        }
    }
}
