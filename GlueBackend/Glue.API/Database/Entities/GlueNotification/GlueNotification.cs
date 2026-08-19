namespace Glue.API.Database.Entities.GlueNotification;

public class GlueNotification : BaseEntity
{
    public required string UserId { get; set; }
    public required string Kind { get; set; }
    public required string Level { get; set; }
    public required string Title { get; set; }
    public string? Message { get; set; }
    public string? ReleateId { get; set; }
    public DateTime? ReadAt { get; set; }
}
