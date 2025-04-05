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

            RuleFor(qa => qa.ServiceCategoryId)
                .GreaterThan(0).WithMessage("ServiceCategoryId must be a valid ID.");

            RuleFor(qa => qa.Options)
                .NotEmpty().WithMessage("At least one option is required.")
                .Must(options => options.Count >= 1).WithMessage("At least one option is required.");

            RuleForEach(qa => qa.Options).ChildRules(option =>
            {
                option.RuleFor(o => o.AnswerText)
                    .NotEmpty().WithMessage("Answer text is required.")
                    .MinimumLength(2).WithMessage("Answer text must be at least 2 characters.");

                option.RuleFor(o => o.ServiceIds)
                    .NotEmpty().WithMessage("At least one service ID is required.")
                    .Must(ids => ids.All(id => id > 0)).WithMessage("All service IDs must be valid.");
            });
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

            RuleFor(qa => qa.Options)
                .NotEmpty().WithMessage("At least one option is required.")
                .Must(options => options.Count >= 1).WithMessage("At least one option is required.");

            RuleForEach(qa => qa.Options).ChildRules(option =>
            {
                option.RuleFor(o => o.QaOptionId)
                    .Must(id => id == null || id > 0).WithMessage("QaOptionId must be null (for new options) or a valid ID.");

                option.RuleFor(o => o.AnswerText)
                    .NotEmpty().WithMessage("Answer text is required.")
                    .MinimumLength(10).WithMessage("Answer text must be at least 10 characters.");

                option.RuleFor(o => o.ServiceIds)
                    .NotEmpty().WithMessage("At least one service ID is required.")
                    .Must(ids => ids.All(id => id > 0)).WithMessage("All service IDs must be valid.");
            });
        }
    }
}