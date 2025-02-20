using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateServiceCategoryDTOValidator : AbstractValidator<CreateServiceCategoryDTO>
    {
        public CreateServiceCategoryDTOValidator()
        {
            RuleFor(sc => sc.Name)
                .NotEmpty().WithMessage("Service category name is required.")
                .MinimumLength(3).WithMessage("Service category name must be at least 3 characters.");

            RuleFor(sc => sc.Status)
                .NotNull().WithMessage("Status must be provided.");

            RuleFor(sc => sc.Exist)
                .NotNull().WithMessage("Exist status must be provided.");
        }
    }

    public class UpdateServiceCategoryDTOValidator : AbstractValidator<UpdateServiceCategoryDTO>
    {
        public UpdateServiceCategoryDTOValidator()
        {
            RuleFor(sc => sc.Name)
                .NotEmpty().WithMessage("Service category name is required.")
                .MinimumLength(3).WithMessage("Service category name must be at least 3 characters.");

            RuleFor(sc => sc.Status)
                .NotNull().WithMessage("Status must be provided.");

            RuleFor(sc => sc.Exist)
                .NotNull().WithMessage("Exist status must be provided.");
        }
    }
}
