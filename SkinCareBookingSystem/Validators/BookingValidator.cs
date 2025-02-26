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

            RuleFor(booking => booking.AppointmentDate)
                .NotEmpty().WithMessage("Appointment date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Appointment date must be in the future.");

            RuleFor(booking => booking.TherapistId)
                .NotNull().WithMessage("Therapist ID is required for specific therapist bookings.");

            RuleFor(booking => booking.Note)
                .MaximumLength(500).WithMessage("Note cannot be longer than 500 characters.");
        }
    }

    public class UpdateBookingDTOValidator : AbstractValidator<UpdateBookingDTO>
    {
        public UpdateBookingDTOValidator()
        {
            RuleFor(booking => booking.Status)
                .NotNull().WithMessage("Status is required.");

            RuleFor(booking => booking.IsPaid)
                .NotNull().WithMessage("Payment status is required.");
        }
    }
}
