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

            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("Image URL must be a valid absolute URL.");
        }
    }

    public class UpdateImageServiceDTOValidator : AbstractValidator<UpdateImageServiceDTO>
    {
        public UpdateImageServiceDTOValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0).WithMessage("ServiceId must be a positive integer.");

            RuleFor(x => x.ImageURL)
                .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Image URL must be a valid absolute URL when provided.");
        }
    }
}