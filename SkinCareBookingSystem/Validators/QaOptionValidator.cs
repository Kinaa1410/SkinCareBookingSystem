using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateQaOptionDTOValidator : AbstractValidator<CreateQaOptionDTO>
    {
        public CreateQaOptionDTOValidator()
        {
            RuleFor(qo => qo.AnswerText)
                .NotEmpty().WithMessage("Answer text is required.")
                .MinimumLength(2).WithMessage("Answer text must be at least 2 characters.");

            RuleFor(qo => qo.ServiceIds)
                .NotEmpty().WithMessage("At least one service ID is required.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All service IDs must be valid.");
        }
    }

    public class UpdateQaOptionDTOValidator : AbstractValidator<UpdateQaOptionDTO>
    {
        public UpdateQaOptionDTOValidator()
        {
            RuleFor(qo => qo.AnswerText)
                .NotEmpty().WithMessage("Answer text is required.")
                .MinimumLength(2).WithMessage("Answer text must be at least 2 characters.");

            RuleFor(qo => qo.ServiceIds)
                .NotEmpty().WithMessage("At least one service ID is required.")
                .Must(ids => ids.All(id => id > 0)).WithMessage("All service IDs must be valid.");
        }
    }
}