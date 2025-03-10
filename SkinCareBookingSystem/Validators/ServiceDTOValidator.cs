using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateServiceDTOValidator : AbstractValidator<CreateServiceDTO>
    {
        public CreateServiceDTOValidator()
        {
            RuleFor(service => service.Name)
                .NotEmpty().WithMessage("Service name is required.");

            RuleFor(service => service.Description)
                .NotEmpty().WithMessage("Service description is required.");

            RuleFor(service => service.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(service => service.Rating)
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.");
        }
    }

    public class UpdateServiceDTOValidator : AbstractValidator<UpdateServiceDTO>
    {
        public UpdateServiceDTOValidator()
        {
            RuleFor(service => service.Name)
                .NotEmpty().WithMessage("Service name is required.");

            RuleFor(service => service.Description)
                .NotEmpty().WithMessage("Service description is required.");

            RuleFor(service => service.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(service => service.Rating)
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.");
        }
    }
}
