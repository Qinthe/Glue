namespace Glue.API.Database.Entities.GlueTask;

public class GlueTaskGroup : BaseEntity
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
}
