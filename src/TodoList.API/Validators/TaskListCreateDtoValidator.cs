using FluentValidation;
using TodoList.API.DTOs;

namespace TodoList.API.Validators;

public class TaskListCreateDtoValidator : AbstractValidator<TaskListCreateDto>
{
    public TaskListCreateDtoValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 100)
            .WithMessage("Name must be between 3 and 100 characters.");
    }
}