using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateRoleDTOValidator : AbstractValidator<CreateRoleDTO>
    {
        public CreateRoleDTOValidator()
        {
            RuleFor(role => role.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(3).WithMessage("Role name must be at least 3 characters.");

            RuleFor(role => role.Status)
                .NotNull().WithMessage("Status must be provided.");
        }
    }

    public class UpdateRoleDTOValidator : AbstractValidator<UpdateRoleDTO>
    {
        public UpdateRoleDTOValidator()
        {
            RuleFor(role => role.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(3).WithMessage("Role name must be at least 3 characters.");

            RuleFor(role => role.Status)
                .NotNull().WithMessage("Status must be provided.");
        }
    }
}
