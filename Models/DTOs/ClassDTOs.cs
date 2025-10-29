namespace StudentManagmentAPI.Models.DTOs
{
    public class CreateClassRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateClassRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class ClassResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EnrolledStudents { get; set; }
    }
}
