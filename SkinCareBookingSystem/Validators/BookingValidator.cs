using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateBookingDTOValidator : AbstractValidator<CreateBookingDTO>
    {
        public CreateBookingDTOValidator()
        {
            RuleFor(booking => booking.UserId)
                .NotEmpty().WithMessage("User ID is required.")
                .GreaterThan(0).WithMessage("Invalid User ID.");

            RuleFor(booking => booking.TherapistId)
                .NotEmpty().WithMessage("Therapist ID is required.")
                .GreaterThan(0).WithMessage("Invalid Therapist ID.");

            RuleFor(booking => booking.ServiceId)
                .NotEmpty().WithMessage("Service ID is required.")
                .GreaterThan(0).WithMessage("Invalid Service ID.");

            RuleFor(booking => booking.TimeSlotId)
                .NotEmpty().WithMessage("Time Slot ID is required.")
                .GreaterThan(0).WithMessage("Invalid Time Slot ID.");

            RuleFor(booking => booking.Note)
                .MaximumLength(500).WithMessage("Note cannot be longer than 500 characters.");

            RuleFor(booking => booking.AppointmentDate)
                .NotEmpty().WithMessage("Appointment date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Appointment date cannot be in the past.");
        }
    }

    public class UpdateBookingDTOValidator : AbstractValidator<UpdateBookingDTO>
    {
        public UpdateBookingDTOValidator()
        {
            RuleFor(booking => booking.Status)
                .IsInEnum().WithMessage("Invalid booking status value.");

            RuleFor(booking => booking.IsPaid)
                .NotNull().WithMessage("Payment status is required.");
        }
    }
}