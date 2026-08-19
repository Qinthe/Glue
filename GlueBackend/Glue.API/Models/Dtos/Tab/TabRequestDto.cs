namespace Glue.API.Models.Dtos.Tab;

public class TabRequestDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public required string Url { get; set; }
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public string? Category { get; set; }
    public required int OpenMode { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsPinned { get; set; }
    public string? Description { get; set; }
    public required string Color { get; set; }
    public DateTime? CreatedAt { get; set; }
}
