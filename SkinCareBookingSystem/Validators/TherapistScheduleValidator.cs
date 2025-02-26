using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateTherapistScheduleDTOValidator : AbstractValidator<CreateTherapistScheduleDTO>
    {
        public CreateTherapistScheduleDTOValidator()
        {
            RuleFor(x => x.TherapistId).NotEmpty().WithMessage("Therapist ID is required.");
            RuleFor(x => x.StartHour).InclusiveBetween(0, 23).WithMessage("Start hour must be between 0 and 23.");
            RuleFor(x => x.StartMinute).InclusiveBetween(0, 59).WithMessage("Start minute must be between 0 and 59.");
            RuleFor(x => x.EndHour).InclusiveBetween(0, 23).WithMessage("End hour must be between 0 and 23.");
            RuleFor(x => x.EndMinute).InclusiveBetween(0, 59).WithMessage("End minute must be between 0 and 59.");
            RuleFor(x => x.EndHour).GreaterThan(x => x.StartHour).WithMessage("End hour must be greater than start hour.");
            RuleFor(x => x.EndMinute).GreaterThan(x => x.StartMinute).WithMessage("End minute must be greater than start minute.");
            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Day of the week is invalid.");
        }
    }

    public class UpdateTherapistScheduleDTOValidator : AbstractValidator<UpdateTherapistScheduleDTO>
    {
        public UpdateTherapistScheduleDTOValidator()
        {
            RuleFor(x => x.StartHour).InclusiveBetween(0, 23).WithMessage("Start hour must be between 0 and 23.");
            RuleFor(x => x.StartMinute).InclusiveBetween(0, 59).WithMessage("Start minute must be between 0 and 59.");
            RuleFor(x => x.EndHour).InclusiveBetween(0, 23).WithMessage("End hour must be between 0 and 23.");
            RuleFor(x => x.EndMinute).InclusiveBetween(0, 59).WithMessage("End minute must be between 0 and 59.");
            RuleFor(x => x.EndHour).GreaterThan(x => x.StartHour).WithMessage("End hour must be greater than start hour.");
            RuleFor(x => x.EndMinute).GreaterThan(x => x.StartMinute).WithMessage("End minute must be greater than start minute.");
            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Day of the week is invalid.");
        }
    }
}
