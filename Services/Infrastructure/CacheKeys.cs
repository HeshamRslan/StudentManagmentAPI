namespace StudentManagementAPI.Services.Infrastructure
{
    public static class CacheKeys
    {
        // Students
        public const string AllStudents = "students:all";
        public static string StudentById(int id) => $"students:id:{id}";
        public const string StudentsPrefix = "students:";

        // Classes
        public const string AllClasses = "classes:all";
        public static string ClassById(int id) => $"classes:id:{id}";
        public static string ClassSearch(string search) => $"classes:search:{search}";
        public const string ClassesPrefix = "classes:";

        // Enrollments
        public const string AllEnrollments = "enrollments:all";
        public static string EnrollmentsByStudent(int studentId) => $"enrollments:student:{studentId}";
        public static string EnrollmentsByClass(int classId) => $"enrollments:class:{classId}";
        public static string EnrollmentCount(int studentId) => $"enrollments:count:student:{studentId}";
        public static string ClassEnrollmentCount(int classId) => $"enrollments:count:class:{classId}";
        public const string EnrollmentsPrefix = "enrollments:";

        // Marks
        public const string AllMarks = "marks:all";
        public static string MarkById(int id) => $"marks:id:{id}";
        public static string MarksByStudent(int studentId) => $"marks:student:{studentId}";
        public static string MarksByClass(int classId) => $"marks:class:{classId}";
        public static string ClassAverage(int classId) => $"marks:average:class:{classId}";
        public const string MarksPrefix = "marks:";

        // Reports (short expiration)
        public static string StudentReport(int studentId) => $"reports:student:{studentId}";
        public const string ReportsPrefix = "reports:";
    }
}