namespace StudentManagmentAPI.Models.DTOs
{
    public class CreateMarkRequest
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public decimal ExamMark { get; set; }
        public decimal AssignmentMark { get; set; }
    }

    public class UpdateMarkRequest
    {
        public int Id { get; set; }
        public decimal ExamMark { get; set; }
        public decimal AssignmentMark { get; set; }
    }
    public class MarkResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public decimal ExamMark { get; set; }
        public decimal AssignmentMark { get; set; }
        public decimal TotalMark { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}