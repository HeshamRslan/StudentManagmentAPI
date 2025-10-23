using FluentValidation;
using StudentManagmentAPI.Models;

public class CreateMarkRequestValidator : AbstractValidator<CreateMarkRequest>
{
    public CreateMarkRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid Student ID is required.");

        RuleFor(x => x.ClassId)
            .GreaterThan(0).WithMessage("Valid Class ID is required.");

        RuleFor(x => x.ExamMark)
            .InclusiveBetween(0, 100).WithMessage("Exam mark must be between 0 and 100.");

        RuleFor(x => x.AssignmentMark)
            .InclusiveBetween(0, 100).WithMessage("Assignment mark must be between 0 and 100.");

        RuleFor(x => x.ExamMark).InclusiveBetween(0, 100).WithMessage("ExamMark must be between 0 and 100.");

        RuleFor(x => x.AssignmentMark).InclusiveBetween(0, 100).WithMessage("AssignmentMark must be between 0 and 100.");

    }
}
