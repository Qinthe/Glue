namespace Glue.API.Database.Entities.GlueTab;

public class GlueTab : BaseEntity
{
    public required string UserId { get; set; }
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public required string Category { get; set; }
    public required int OpenMode { get; set; }
    public int SortOrder { get; set; } = 0;
    public required bool IsPinned { get; set; } = false;
    public string? Description { get; set; }
    public string? Color { get; set; }
}
