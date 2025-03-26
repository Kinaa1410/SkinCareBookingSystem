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
                .Must(ao => ao == "Yes" || ao == "No")
                .WithMessage("AnswerOption must be either 'Yes' or 'No'.");

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