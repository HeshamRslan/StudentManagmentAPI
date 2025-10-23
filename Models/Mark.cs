namespace StudentManagmentAPI.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public decimal  ExamMark { get; set; }
        public decimal AssignmentMark { get; set; }
        public decimal TotalMark => ExamMark + AssignmentMark;
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
            
    }
}
