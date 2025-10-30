using FluentValidation;
using StudentManagmentAPI.Models.DTOs;

public class UpdateMarkRequestValidator : AbstractValidator<UpdateMarkRequest>
{
    public UpdateMarkRequestValidator()
    {
        RuleFor(x => x.ExamMark)
            .InclusiveBetween(0, 100)
            .WithMessage("Exam mark must be between 0 and 100.");

        RuleFor(x => x.AssignmentMark)
            .InclusiveBetween(0, 100)
            .WithMessage("Assignment mark must be between 0 and 100.");
    }
}