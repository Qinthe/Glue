namespace Glue.API.Models.Dtos.Task;

public class TaskWorkbenchDto
{
    public required string GroupId { get; set; }
    public required string GroupName { get; set; }

    public IEnumerable<TaskDto> Tasks { get; set; } = [];
}
