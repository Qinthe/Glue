namespace Glue.API.Database.Entities.GlueMemoNote;

public class GlueMemoTag : BaseEntity
{
    public required string MemoId { get; set; }
    public required string Tag { get; set; }
}
