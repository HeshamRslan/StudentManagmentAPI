using System;
using StudentManagmentAPI.Models;
using StudentManagmentAPI.Models.DTOs;

namespace StudentManagementAPI.Mappings
{
    public static class ClassMapping
    {
        public static Class ToModel(this CreateClassRequest r) => new Class
        {
            Name = r.Name,
            Teacher = r.Teacher,
            Description = r.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        public static ClassResponse ToResponse(this Class c, int enrolledCount = 0) => new ClassResponse
        {
            Id = c.Id,
            Name = c.Name,
            Teacher = c.Teacher,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            EnrolledStudents = enrolledCount
        };
    }
}
