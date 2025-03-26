using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateQaAnswerDTOValidator : AbstractValidator<CreateQaAnswerDTO>
    {
        public CreateQaAnswerDTOValidator()
        {
            RuleFor(qaAnswer => qaAnswer.Answer)
                .NotEmpty().WithMessage("Answer is required.")
                .Must(a => a.Equals("Yes", StringComparison.OrdinalIgnoreCase) || a.Equals("No", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Answer must be 'Yes' or 'No'.");
        }
    }

    public class UpdateQaAnswerDTOValidator : AbstractValidator<UpdateQaAnswerDTO>
    {
        public UpdateQaAnswerDTOValidator()
        {
            RuleFor(qaAnswer => qaAnswer.Answer)
                .NotEmpty().WithMessage("Answer is required.")
                .Must(a => a.Equals("Yes", StringComparison.OrdinalIgnoreCase) || a.Equals("No", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Answer must be 'Yes' or 'No'.");
        }
    }
}