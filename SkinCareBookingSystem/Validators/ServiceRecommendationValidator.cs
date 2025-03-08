using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateServiceRecommendationDTOValidator : AbstractValidator<CreateServiceRecommendationDTO>
    {
        public CreateServiceRecommendationDTOValidator()
        {
            RuleFor(sr => sr.QaId)
                .GreaterThan(0).WithMessage("QaId must be greater than 0.");

            RuleFor(sr => sr.AnswerOption)
                .NotEmpty().WithMessage("AnswerOption is required.")
                .Length(1).WithMessage("AnswerOption must be a single character (A, B, C, D).");

            RuleFor(sr => sr.ServiceId)
                .GreaterThan(0).WithMessage("ServiceId must be greater than 0.");

            RuleFor(sr => sr.Weight)
                .GreaterThan(0).WithMessage("Weight must be a positive value.");
        }
    }

    public class UpdateServiceRecommendationDTOValidator : AbstractValidator<UpdateServiceRecommendationDTO>
    {
        public UpdateServiceRecommendationDTOValidator()
        {
            RuleFor(sr => sr.Weight)
                .GreaterThan(0).WithMessage("Weight must be a positive value.");
        }
    }
}
