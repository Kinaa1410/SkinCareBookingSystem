using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateFeedbackDTOValidator : AbstractValidator<CreateFeedbackDTO>
    {
        public CreateFeedbackDTOValidator()
        {
            RuleFor(feedback => feedback.BookingId)
                .NotEmpty().WithMessage("Booking ID is required.")
                .GreaterThan(0).WithMessage("Invalid Booking ID.");

            RuleFor(feedback => feedback.RatingService)
                .InclusiveBetween(1, 5).WithMessage("Rating for Service must be between 1 and 5.");

            RuleFor(feedback => feedback.RatingTherapist)
                .InclusiveBetween(1, 5).WithMessage("Rating for Therapist must be between 1 and 5.");

            RuleFor(feedback => feedback.CommentService)
                .NotEmpty().WithMessage("Service comment is required.")
                .MaximumLength(500).WithMessage("Service comment cannot be longer than 500 characters.");

            RuleFor(feedback => feedback.CommentTherapist)
                .NotEmpty().WithMessage("Therapist comment is required.")
                .MaximumLength(500).WithMessage("Therapist comment cannot be longer than 500 characters.");
        }
    }

    public class UpdateFeedbackDTOValidator : AbstractValidator<UpdateFeedbackDTO>
    {
        public UpdateFeedbackDTOValidator()
        {
            RuleFor(feedback => feedback.RatingService)
                .InclusiveBetween(1, 5).WithMessage("Rating for Service must be between 1 and 5.");

            RuleFor(feedback => feedback.RatingTherapist)
                .InclusiveBetween(1, 5).WithMessage("Rating for Therapist must be between 1 and 5.");

            RuleFor(feedback => feedback.CommentService)
                .MaximumLength(500).WithMessage("Service comment cannot be longer than 500 characters.");

            RuleFor(feedback => feedback.CommentTherapist)
                .MaximumLength(500).WithMessage("Therapist comment cannot be longer than 500 characters.");

            RuleFor(feedback => feedback.Status)
                .NotNull().WithMessage("Status is required.");
        }
    }
}
