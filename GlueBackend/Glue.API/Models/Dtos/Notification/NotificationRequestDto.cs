namespace Glue.API.Models.Dtos.Notification;

public class NotificationRequestDto
{
    public required string Kind { get; set; }
    public required string Level { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public string? ReleateId { get; set; }
}
