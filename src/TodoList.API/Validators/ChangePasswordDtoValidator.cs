using FluentValidation;
using TodoList.API.DTOs;

namespace TodoList.API.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(cp => cp.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(cp => cp.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6)
            .WithMessage("New password must be at least 6 characters.");

        RuleFor(cp => cp.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm new password is required.")
            .Equal(c => c.NewPassword)
            .WithMessage("New password and confirm new password must match.");
    }
}