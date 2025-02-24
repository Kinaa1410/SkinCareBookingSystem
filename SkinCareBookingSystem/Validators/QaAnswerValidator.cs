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
                .MinimumLength(5).WithMessage("Answer must be at least 5 characters.");
        }
    }

    public class UpdateQaAnswerDTOValidator : AbstractValidator<UpdateQaAnswerDTO>
    {
        public UpdateQaAnswerDTOValidator()
        {
            RuleFor(qaAnswer => qaAnswer.Answer)
                .NotEmpty().WithMessage("Answer is required.")
                .MinimumLength(5).WithMessage("Answer must be at least 5 characters.");
        }
    }
}
