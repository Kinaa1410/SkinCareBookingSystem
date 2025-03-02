using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateTherapistTimeSlotDTOValidator : AbstractValidator<CreateTherapistTimeSlotDTO>
    {
        public CreateTherapistTimeSlotDTOValidator()
        {
            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

            RuleFor(x => x.IsAvailable)
                .NotNull().WithMessage("Availability status is required.");
        }
    }

    public class UpdateTherapistTimeSlotDTOValidator : AbstractValidator<UpdateTherapistTimeSlotDTO>
    {
        public UpdateTherapistTimeSlotDTOValidator()
        {
            RuleFor(x => x.TimeSlotId).GreaterThan(0).WithMessage("TimeSlotId must be a valid ID.");

            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

            RuleFor(x => x.IsAvailable)
                .NotNull().WithMessage("Availability status is required.");
        }
    }
}
