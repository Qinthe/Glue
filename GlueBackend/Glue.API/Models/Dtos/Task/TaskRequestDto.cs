namespace Glue.API.Models.Dtos.Task;

public class TaskRequestDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int Progress { get; set; }
    public required string Status { get; set; }
    public bool ReminderEnabled { get; set; }
    public int ReminderMinutesBefore { get; set; }
    public DateTime? LastReminderAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
