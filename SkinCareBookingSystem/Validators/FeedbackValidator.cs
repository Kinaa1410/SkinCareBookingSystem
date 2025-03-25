using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateFeedbackDTOValidator : AbstractValidator<CreateFeedbackDTO>
    {
        public CreateFeedbackDTOValidator()
        {
            RuleFor(feedback => feedback.ServiceId)
                .NotEmpty().WithMessage("Service ID is required.")
                .GreaterThan(0).WithMessage("Invalid Service ID.");

            RuleFor(feedback => feedback.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(feedback => feedback.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(500).WithMessage("Comment cannot be longer than 500 characters.");
        }
    }

    public class UpdateFeedbackDTOValidator : AbstractValidator<UpdateFeedbackDTO>
    {
        public UpdateFeedbackDTOValidator()
        {
            RuleFor(feedback => feedback.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(feedback => feedback.Comment)
                .MaximumLength(500).WithMessage("Comment cannot be longer than 500 characters.");
        }
    }
}