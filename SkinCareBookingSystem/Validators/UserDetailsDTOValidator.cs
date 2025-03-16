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

            // FirstName is required for creation
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .When(x => !string.IsNullOrEmpty(x.FirstName)) // Only validate if provided
                .MinimumLength(2).WithMessage("First name must be at least 2 characters.");

            // LastName is required for creation
            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .When(x => !string.IsNullOrEmpty(x.LastName)) // Only validate if provided
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");

            // Address is required for creation
            RuleFor(user => user.Address)
                .NotEmpty().WithMessage("Address is required.")
                .When(x => !string.IsNullOrEmpty(x.Address)); // Only validate if provided

            // Gender is required for creation
            RuleFor(user => user.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(g => g == "Male" || g == "Female" || g == "Other")
                .WithMessage("Gender must be 'Male', 'Female', or 'Other'.")
                .When(x => !string.IsNullOrEmpty(x.Gender)); // Only validate if provided

            // avatarFile is optional for creation, no validation if not provided
            RuleFor(user => user.Avatar)
                .NotEmpty().WithMessage("Avatar file is required.")
                .When(x => !string.IsNullOrEmpty(x.Avatar)); // Only validate if avatar is provided
        }
    }

    //public class UpdateUserDetailsDTOValidator : AbstractValidator<UpdateUserDetailsDTO>
    //{
    //    public UpdateUserDetailsDTOValidator()
    //    {
    //        // FirstName is optional for update but required if provided
    //        RuleFor(user => user.FirstName)
    //            .NotEmpty().WithMessage("First name is required.")
    //            .When(x => !string.IsNullOrEmpty(x.FirstName)) // Only validate if provided
    //            .MinimumLength(2).WithMessage("First name must be at least 2 characters.");

    //        // LastName is optional for update but required if provided
    //        RuleFor(user => user.LastName)
    //            .NotEmpty().WithMessage("Last name is required.")
    //            .When(x => !string.IsNullOrEmpty(x.LastName)) // Only validate if provided
    //            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.");

    //        // Address is optional for update but required if provided
    //        RuleFor(user => user.Address)
    //            .NotEmpty().WithMessage("Address is required.")
    //            .When(x => !string.IsNullOrEmpty(x.Address)); // Only validate if provided

    //        // Gender is optional for update but required if provided
    //        RuleFor(user => user.Gender)
    //            .NotEmpty().WithMessage("Gender is required.")
    //            .Must(g => g == "Male" || g == "Female" || g == "Other")
    //            .WithMessage("Gender must be 'Male', 'Female', or 'Other'.")
    //            .When(x => !string.IsNullOrEmpty(x.Gender)); // Only validate if provided

    //        // avatarFile is optional for update
    //        RuleFor(user => user.Avatar)
    //            .NotEmpty().WithMessage("Avatar file is required.")
    //            .When(x => !string.IsNullOrEmpty(x.Avatar)); // Only validate if avatar is provided
    //    }
    //}
}
