using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateCartItemDTOValidator : AbstractValidator<CreateCartItemDTO>
    {
        public CreateCartItemDTOValidator()
        {
            RuleFor(cartItem => cartItem.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(cartItem => cartItem.ServiceId)
                .NotEmpty().WithMessage("Service ID is required.");
        }
    }

    public class UpdateCartItemDTOValidator : AbstractValidator<UpdateCartItemDTO>
    {
        public UpdateCartItemDTOValidator()
        {
            RuleFor(cartItem => cartItem.ServiceId)
                .NotEmpty().WithMessage("Service ID is required.");
        }
    }
}
