using FluentValidation;
using StudentManagmentAPI.Models;
using StudentManagmentAPI.Models.DTOs;

public class CreateEnrollmentRequestValidator : AbstractValidator<EnrollRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid Student ID is required.");

        RuleFor(x => x.ClassId)
            .GreaterThan(0).WithMessage("Valid Class ID is required.");

    }
}
