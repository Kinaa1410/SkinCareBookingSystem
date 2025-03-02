using FluentValidation;
using SkinCareBookingSystem.DTOs;

public class CreateTherapistTimeSlotDTOValidator : AbstractValidator<CreateTherapistTimeSlotDTO>
{
    public CreateTherapistTimeSlotDTOValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be later than start time.");

        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Availability status is required.");
    }

    
}

public class UpdateTherapistTimeSlotDTOValidator : AbstractValidator<UpdateTherapistTimeSlotDTO>
{
    public UpdateTherapistTimeSlotDTOValidator()
    {
        RuleFor(x => x.TimeSlotId)
            .GreaterThan(0).WithMessage("Time Slot ID must be valid.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be later than start time.");

        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Availability status is required.");
    }
}

