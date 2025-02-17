using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateUserDetailsDTOValidator : AbstractValidator<CreateUserDetailsDTO>
    {
        public CreateUserDetailsDTOValidator()
        {
            RuleFor(user => user.UserId)
                .GreaterThan(0).WithMessage("UserId must be a valid ID.");

            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");

            RuleFor(user => user.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(user => user.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .WithMessage("Gender must be 'Male', 'Female', or 'Other'.");

            RuleFor(user => user.Avatar)
                .Must(a => string.IsNullOrEmpty(a) || Uri.IsWellFormedUriString(a, UriKind.Absolute))
                .WithMessage("Avatar must be a valid URL if provided.");
        }
    }

    public class UpdateUserDetailsDTOValidator : AbstractValidator<UpdateUserDetailsDTO>
    {
        public UpdateUserDetailsDTOValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(2).WithMessage("First name must be at least 2 characters.");

            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");

            RuleFor(user => user.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(user => user.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .WithMessage("Gender must be 'Male', 'Female', or 'Other'.");

            RuleFor(user => user.Avatar)
                .Must(a => string.IsNullOrEmpty(a) || Uri.IsWellFormedUriString(a, UriKind.Absolute))
                .WithMessage("Avatar must be a valid URL if provided.");
        }
    }
}
