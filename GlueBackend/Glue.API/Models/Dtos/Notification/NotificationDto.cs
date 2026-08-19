namespace Glue.API.Models.Dtos.Notification;

public class NotificationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Kind { get; set; } 
    public required string Level { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public string? ReleateId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
