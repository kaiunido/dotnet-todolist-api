using FluentValidation;
using TodoList.API.DTOs;

namespace TodoList.API.Validators;

public class TaskItemCreateDtoValidator : AbstractValidator<TaskItemCreateDto>
{
    public TaskItemCreateDtoValidator()
    {
        RuleFor(t => t.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(3, 255)
            .WithMessage("Description must be between 3 and 255 characters.");
    }
}