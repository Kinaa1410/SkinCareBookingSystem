using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateQaDTOValidator : AbstractValidator<CreateQaDTO>
    {
        public CreateQaDTOValidator()
        {
            RuleFor(qa => qa.Question)
                .NotEmpty().WithMessage("Question is required.")
                .MinimumLength(5).WithMessage("Question must be at least 5 characters.");

            RuleFor(qa => qa.Type)
                .NotEmpty().WithMessage("Type is required.");

            RuleFor(qa => qa.Status)
                .NotNull().WithMessage("Status must be provided.");
        }
    }

    public class UpdateQaDTOValidator : AbstractValidator<UpdateQaDTO>
    {
        public UpdateQaDTOValidator()
        {
            RuleFor(qa => qa.Question)
                .NotEmpty().WithMessage("Question is required.")
                .MinimumLength(5).WithMessage("Question must be at least 5 characters.");

            RuleFor(qa => qa.Type)
                .NotEmpty().WithMessage("Type is required.");

            RuleFor(qa => qa.Status)
                .NotNull().WithMessage("Status must be provided.");
        }
    }
}
