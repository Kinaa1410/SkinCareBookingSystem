using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateImageServiceDTOValidator : AbstractValidator<CreateImageServiceDTO>
    {
        public CreateImageServiceDTOValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("ServiceId must be a positive integer.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Image file is required.")
                .Must(file => file.ContentType.Contains("image")).WithMessage("The file must be an image.");
        }
    }

    public class UpdateImageServiceDTOValidator : AbstractValidator<UpdateImageServiceDTO>
    {
        public UpdateImageServiceDTOValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("ServiceId must be a positive integer.");

            RuleFor(x => x.ImageFile)
                .Must(file => file == null || file.ContentType.Contains("image")).WithMessage("The file must be an image.");
        }
    }
}
