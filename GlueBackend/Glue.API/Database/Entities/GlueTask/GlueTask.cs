namespace Glue.API.Database.Entities.GlueTask;

public class GlueTask : BaseEntity
{
    public required string UserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public required DateTime? StartAt { get; set; }
    public required DateTime? EndAt { get; set; }
    public required int Progress { get; set; } = 0;
    public required string Status { get; set; } = "pending";
    public required bool ReminderEnabled { get; set; } = true;
    public required int ReminderMinutesBefore { get; set; } = 30;

    public DateTime? LastReminderAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
