using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateBookingDTOValidator : AbstractValidator<CreateBookingDTO>
    {
        public CreateBookingDTOValidator()
        {
            RuleFor(booking => booking.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(booking => booking.TimeSlotId)
                .NotEmpty().WithMessage("Time Slot ID is required.")
                .GreaterThan(0).WithMessage("Invalid Time Slot ID.");

            RuleFor(booking => booking.TherapistId)
                .GreaterThan(0).When(booking => booking.TherapistId.HasValue)
                .WithMessage("Invalid Therapist ID.");

            RuleFor(booking => booking.Note)
                .MaximumLength(500).WithMessage("Note cannot be longer than 500 characters.");
        }
    }

    public class UpdateBookingDTOValidator : AbstractValidator<UpdateBookingDTO>
    {
        public UpdateBookingDTOValidator()
        {
            RuleFor(booking => booking.Status)
                .IsInEnum().WithMessage("Invalid booking status value."); // Validates against BookingStatus enum

            RuleFor(booking => booking.IsPaid)
                .NotNull().WithMessage("Payment status is required.");
        }
    }
}