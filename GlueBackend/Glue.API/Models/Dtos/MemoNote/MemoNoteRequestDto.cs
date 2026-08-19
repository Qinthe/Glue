namespace Glue.API.Models.Dtos.MemoNote;

public class MemoNoteRequestDto
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Category { get; set; }
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
}
