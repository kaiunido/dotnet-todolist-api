using TodoList.API.Models;

namespace TodoList.API.Tests.Unit.Models;

public class TaskItemTests
{
    [Fact]
    public void Ctor_ShouldNormalizeDescription()
    {
        var task = new TaskItem(1, "  test description  ");

        Assert.Equal("test description", task.Description);
        Assert.False(task.IsDone);
        Assert.Null(task.DoneAt);
    }

    [Fact]
    public void Ctor_WithEmptyDescription_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TaskItem(1, "   "));
    }

    [Fact]
    public void UpdateDescription_WithSameValueAfterTrim_ShouldNotChange()
    {
        var task = new TaskItem(1, "test description");
        var oldUpdatedAt = task.UpdatedAt;

        task.UpdateDescription(" test description  ");

        Assert.Equal("test description", task.Description);
        Assert.Equal(oldUpdatedAt, task.UpdatedAt);
    }

    [Fact]
    public void UpdateDescription_WithEmpty_ShouldThrowArgumentException()
    {
        var task = new TaskItem(1, "test description");

        Assert.Throws<ArgumentException>(() => task.UpdateDescription("   "));
    }

    [Fact]
    public void MarkAsDone_ShouldSetDoneAt()
    {
        var task = new TaskItem(1, "test description");

        task.MarkAsDone();

        Assert.True(task.IsDone);
        Assert.NotNull(task.DoneAt);
    }

    [Fact]
    public void MarkAsDone_WhenAlreadyDone_ShouldNotChangeDoneAt()
    {
        var task = new TaskItem(1, "test description", true);
        var firstDoneAt = task.DoneAt;

        task.MarkAsDone();

        Assert.True(task.IsDone);
        Assert.Equal(firstDoneAt, task.DoneAt);
    }

    [Fact]
    public void UnmarkAsDone_ShouldClearDoneAt()
    {
        var task = new TaskItem(1, "test descriptions", true);

        task.UnmarkAsDone();

        Assert.False(task.IsDone);
        Assert.Null(task.DoneAt);
    }
}