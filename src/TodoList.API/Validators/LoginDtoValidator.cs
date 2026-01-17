using FluentValidation;
using TodoList.API.DTOs;

namespace TodoList.API.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(l => l.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}