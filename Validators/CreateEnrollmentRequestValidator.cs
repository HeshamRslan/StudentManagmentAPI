using FluentValidation;
using StudentManagmentAPI.Models;

public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid Student ID is required.");

        RuleFor(x => x.ClassId)
            .GreaterThan(0).WithMessage("Valid Class ID is required.");
    }
}
