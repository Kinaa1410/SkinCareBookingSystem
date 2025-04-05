using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateQaAnswerDTOValidator : AbstractValidator<CreateQaAnswerDTO>
    {
        public CreateQaAnswerDTOValidator()
        {
            RuleFor(qaAnswer => qaAnswer.UserId)
                .GreaterThan(0).WithMessage("UserId must be a valid ID.");

            RuleFor(qaAnswer => qaAnswer.QaId)
                .GreaterThan(0).WithMessage("QaId must be a valid ID.");

            RuleFor(qaAnswer => qaAnswer.QaOptionId)
                .GreaterThan(0).WithMessage("QaOptionId must be a valid option ID.");
        }
    }

    public class UpdateQaAnswerDTOValidator : AbstractValidator<UpdateQaAnswerDTO>
    {
        public UpdateQaAnswerDTOValidator()
        {
            RuleFor(qaAnswer => qaAnswer.QaOptionId)
                .GreaterThan(0).WithMessage("QaOptionId must be a valid option ID.");
        }
    }
}