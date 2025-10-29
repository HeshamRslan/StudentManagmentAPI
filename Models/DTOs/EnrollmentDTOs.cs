namespace StudentManagmentAPI.Models.DTOs
{
    public class EnrollRequest
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
    }

    public class EnrollmentResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}