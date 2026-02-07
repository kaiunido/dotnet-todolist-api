using FluentValidation.TestHelper;
using TodoList.API.DTOs;
using TodoList.API.Validators;

namespace TodoList.API.Tests.Validators;

public class TaskListCreateDtoValidatorTests
{
    private readonly TaskListCreateDtoValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Valid()
    {
        var dto = new TaskListCreateDto { Name = "Valid Name" };
        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(tl => tl.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ab")]
    public void Should_Have_Error_When_Name_Is_Invalid(string name)
    {
        var dto = new TaskListCreateDto { Name = name };
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(tl => tl.Name);
    }
}