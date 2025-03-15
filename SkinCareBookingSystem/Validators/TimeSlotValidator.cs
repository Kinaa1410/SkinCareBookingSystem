using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateTimeSlotDTOValidator : AbstractValidator<CreateTimeSlotDTO>
    {
        public CreateTimeSlotDTOValidator()
        {
            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        }
    }

    public class UpdateTimeSlotDTOValidator : AbstractValidator<UpdateTimeSlotDTO>
    {
        public UpdateTimeSlotDTOValidator()
        {
            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime).WithMessage("Start time must be before end time.");
        }
    }
}
