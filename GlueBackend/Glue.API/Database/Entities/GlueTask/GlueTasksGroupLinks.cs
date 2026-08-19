namespace Glue.API.Database.Entities.GlueTask;

public class GlueTasksGroupLinks : BaseEntity
{
    public required string GroupId { get; set; } = Guid.NewGuid().ToString("N");
    public required string Name { get; set; }
    public required string Color { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
}
