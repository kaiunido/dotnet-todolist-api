using FluentValidation.TestHelper;
using TodoList.API.DTOs;
using TodoList.API.Validators;

namespace TodoList.API.Tests.Validators;

public class TaskItemCreateDtoValidatorTests
{
    private readonly TaskItemCreateDtoValidator _validator = new();

    public static TheoryData<string?> InvalidDescriptions =>
    [
        null,
        "",
        "   ",
        "ab",
        new('a', 256)
    ];

    [Fact]
    public void ShouldNotHaveErrorWhenDescriptionIsValid()
    {
        var dto = new TaskItemCreateDto { Description = "Test description" };
        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(ti => ti.Description);
    }

    [Theory]
    [MemberData(nameof(InvalidDescriptions))]
    public void ShouldHaveErrorWhenDescriptionIsEmpty(string? description)
    {
        var dto = new TaskItemCreateDto { Description = description };
        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(ti => ti.Description);
    }
}