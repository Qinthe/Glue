namespace Glue.API.Models.Dtos.Task;

public class TaskGroupDto
{
    public required string Id { get; set; } 
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public DateTime? CreatedDate { get; set; }
    public TaskDto? Tasks { get; set; }
}
