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
                .NotEmpty().WithMessage("Avatar URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Avatar must be a valid absolute URL.");
        }
    }

    //public class UpdateUserDetailsDTOValidator : AbstractValidator<UpdateUserDetailsDTO>
    //{
    //    public UpdateUserDetailsDTOValidator()
    //    {
    //        RuleFor(user => user.UserId)
    //            .GreaterThan(0).WithMessage("UserId must be a valid ID.");

    //        RuleFor(user => user.FirstName)
    //            .NotEmpty().WithMessage("First name cannot be empty when provided.")
    //            .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
    //            .When(x => !string.IsNullOrEmpty(x.FirstName));

    //        RuleFor(user => user.LastName)
    //            .NotEmpty().WithMessage("Last name cannot be empty when provided.")
    //            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
    //            .When(x => !string.IsNullOrEmpty(x.LastName));

    //        RuleFor(user => user.Address)
    //            .NotEmpty().WithMessage("Address cannot be empty when provided.")
    //            .When(x => !string.IsNullOrEmpty(x.Address));

    //        RuleFor(user => user.Gender)
    //            .NotEmpty().WithMessage("Gender cannot be empty when provided.")
    //            .Must(g => g == "Male" || g == "Female" || g == "Other")
    //            .WithMessage("Gender must be 'Male', 'Female', or 'Other'.")
    //            .When(x => !string.IsNullOrEmpty(x.Gender));

    //        RuleFor(user => user.Avatar)
    //            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
    //            .WithMessage("Avatar must be a valid absolute URL when provided.");
    //    }
    //}
}