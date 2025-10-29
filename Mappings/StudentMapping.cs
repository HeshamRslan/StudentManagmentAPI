using StudentManagmentAPI.Models;
using StudentManagmentAPI.Models.DTOs;
namespace StudentManagementAPI.Mappings
{
    public static class StudentMapping
    {
        public static Student ToModel(this CreateStudentRequest r) => new Student
        {
            FirstName = r.FirstName,
            LastName = r.LastName
        };

        public static StudentResponse ToResponse(this Student s) => new StudentResponse
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName
        };
    }
}
