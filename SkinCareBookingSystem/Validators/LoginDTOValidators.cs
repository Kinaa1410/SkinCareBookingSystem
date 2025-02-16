using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username cannot be empty!");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be empty!")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long!");
        }
    }
}
