namespace Glue.API.Database.Entities.GlueMemoNote;

public class GlueMemoNote : BaseEntity
{
    public required string UserId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Category { get; set; }

    public IEnumerable<GlueMemoTag> Tags { get; set; } = Enumerable.Empty<GlueMemoTag>();
}
