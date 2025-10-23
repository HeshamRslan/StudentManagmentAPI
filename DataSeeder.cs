using StudentManagmentAPI.Models;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.SeedData
{
    public static class DataSeeder
    {
        public static void Seed(
            StudentService studentService,
            ClassService classService,
            EnrollmentService enrollmentService,
            MarkService markService)
        {
            // ✅ 10 طلاب
            var students = new[]
            {
                new Student { FirstName = "Hesham", LastName = "Rslan" },
                new Student { FirstName = "Omar", LastName = " Khaled" },
                new Student {FirstName = "Aya", LastName = "Hassan"},
                new Student {FirstName = "Laila", LastName = "Ahmed"},
                new Student {FirstName = "Mostafa", LastName = "Ali"},
                new Student {FirstName = "Sara", LastName = "Youssef"},
                new Student {FirstName = "Yassin", LastName = "Gamal"},
                new Student {FirstName = "Nour", LastName = "Adel"},
                new Student {FirstName = "Khaled", LastName = "Tamer"},
                new Student {FirstName = "Dina", LastName = "Hassan"}
            };
            foreach (var s in students)
                studentService.Add(s);

            
            var classes = new[]
            {
                new Class { Name = "Mathematics", Teacher = "Mr. Ali", Description = "Basic Algebra", CreatedAt = DateTime.UtcNow.AddMonths(-7) }, 
                new Class { Name = "Science", Teacher = "Dr. Mona", Description = "General Science", CreatedAt = DateTime.UtcNow.AddMonths(-1) },
                new Class { Name = "History", Teacher = "Mr. Karim", Description = "World History", CreatedAt = DateTime.UtcNow.AddMonths(-2) },
                new Class { Name = "Programming", Teacher = "Mr. Tarek", Description = "Intro to C#", CreatedAt = DateTime.UtcNow.AddMonths(-3) },
                new Class { Name = "English", Teacher = "Ms. Noura", Description = "Language & Grammar", CreatedAt = DateTime.UtcNow.AddMonths(-5) },
                new Class { Name = "Physics", Teacher = "Dr. Hany", Description = "Intro to Mechanics", CreatedAt = DateTime.UtcNow.AddDays(-20) }
            };
            foreach (var c in classes)
                classService.Add(c);

            //  تسجيلات الطلاب في الكلاسات (Enrollments)
            var enrollPairs = new (int s, int c)[]
            {
                (0,0),(0,1),(0,2),
                (1,0),(1,1),(1,3),
                (2,1),(2,2),(2,4),
                (3,2),(3,3),(3,4),
                (4,0),(4,1),(4,5),
                (5,2),(5,4),(5,5),
                (6,1),(6,3),
                (7,3),(7,5),
                (8,0),(8,5),
                (9,4)
            };

            foreach (var (s, c) in enrollPairs)
            {
                enrollmentService.TryEnrollAsync(
                    students[s].Id,
                    classes[c].Id,
                    sid => enrollmentService.GetByStudentId(sid).Count(),
                    cid => enrollmentService.GetByClassId(cid).Count()
                ).Wait();
            }

            var marks = new List<Mark>();

            void AddMarks(int s, int c, params (decimal exam, decimal assign, int daysAgo)[] m)
            {
                foreach (var x in m)
                {
                    marks.Add(new Mark
                    {
                        StudentId = students[s].Id,
                        ClassId = classes[c].Id,
                        ExamMark = x.exam,
                        AssignmentMark = x.assign,
                        RecordedAt = DateTime.UtcNow.AddDays(-x.daysAgo)
                    });
                }
            }

            // Mathematics
            AddMarks(0, 0, (80, 10, 60), (85, 10, 20), (90, 5, 5));
            AddMarks(1, 0, (75, 15, 40), (78, 10, 10));
            AddMarks(4, 0, (60, 20, 15));
            AddMarks(8, 0, (92, 8, 7));

            // Science
            AddMarks(0, 1, (90, 5, 10));
            AddMarks(1, 1, (85, 10, 5));
            AddMarks(2, 1, (70, 15, 4));
            AddMarks(4, 1, (88, 10, 3));
            AddMarks(6, 1, (77, 10, 1));

            // History
            AddMarks(2, 2, (65, 20, 15), (75, 10, 5));
            AddMarks(3, 2, (55, 25, 20));
            AddMarks(5, 2, (85, 5, 7));

            // Programming
            AddMarks(1, 3, (95, 4, 2));
            AddMarks(3, 3, (93, 6, 1));
            AddMarks(6, 3, (70, 10, 10));
            AddMarks(7, 3, (88, 7, 5));

            // English
            AddMarks(2, 4, (80, 10, 3));
            AddMarks(3, 4, (77, 12, 4));
            AddMarks(5, 4, (90, 7, 2));
            AddMarks(9, 4, (95, 5, 1));

            // Physics
            AddMarks(4, 5, (70, 15, 3));
            AddMarks(5, 5, (88, 6, 2));
            AddMarks(7, 5, (83, 8, 1));
            AddMarks(8, 5, (91, 7, 1));

            foreach (var m in marks)
                markService.Add(m);
        }
    }
}
