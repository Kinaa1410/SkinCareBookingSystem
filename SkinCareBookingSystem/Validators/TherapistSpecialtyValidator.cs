using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class TherapistSpecialtyValidator : AbstractValidator<TherapistSpecialtyDTO>
    {
        public TherapistSpecialtyValidator()
        {
            RuleFor(x => x.TherapistId).NotEmpty().WithMessage("Therapist ID is required.");
            RuleFor(x => x.ServiceCategoryId).NotEmpty().WithMessage("Service Category ID is required.");
        }
    }
}
