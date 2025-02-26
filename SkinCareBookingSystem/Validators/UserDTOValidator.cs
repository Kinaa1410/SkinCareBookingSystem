using FluentValidation;
using SkinCareBookingSystem.DTOs;

namespace SkinCareBookingSystem.Validators
{
    public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidator()
        {
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage("Tên người dùng không được bỏ trống!")
                .MinimumLength(2).WithMessage("Tên phải có ít nhất 2 ký tự!");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email không được bỏ trống!")
                .EmailAddress().WithMessage("Sai định dạng Email.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Mật khẩu không được bỏ trống!")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");
        }
    }

    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserDTOValidator()
        {
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage("Tên người dùng không được bỏ trống!");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email không được bỏ trống!")
                .EmailAddress().WithMessage("Sai định dạng Email.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Mật khẩu không được bỏ trống!")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");

        }
    }
}
