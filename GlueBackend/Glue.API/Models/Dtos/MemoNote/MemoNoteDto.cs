namespace Glue.API.Models.Dtos.MemoNote;

public class MemoNoteDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Category { get; set; }
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
