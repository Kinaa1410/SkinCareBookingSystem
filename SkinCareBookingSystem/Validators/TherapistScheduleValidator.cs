using FluentValidation;
using SkinCareBookingSystem.DTOs;

public class CreateTherapistScheduleDTOValidator : AbstractValidator<CreateTherapistScheduleDTO>
{
    public CreateTherapistScheduleDTOValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .Must(startTime => TimeSpan.TryParse(startTime, out _)).WithMessage("Invalid start time format.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .Must(endTime => TimeSpan.TryParse(endTime, out _)).WithMessage("Invalid end time format.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be later than start time.");

        RuleFor(x => x.TherapistId)
            .GreaterThan(0).WithMessage("Therapist ID must be valid.");
    }
}

public class UpdateTherapistScheduleDTOValidator : AbstractValidator<UpdateTherapistScheduleDTO>
{
    public UpdateTherapistScheduleDTOValidator()
    {
        RuleFor(x => x.TimeSlots)
            .NotEmpty().WithMessage("At least one time slot is required.");
    }
}
