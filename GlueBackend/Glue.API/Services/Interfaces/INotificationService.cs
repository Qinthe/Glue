using Glue.API.Models.Dtos.Notification;

namespace Glue.API.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool? onlyUnread = null, string? kind = null);
    Task<int> GetUnreadCountAsync(string userId);
    Task<NotificationDto> CreateAsync(string userId, NotificationRequestDto requestDto);
    Task<NotificationDto?> UpsertAsync(string id, string userId, NotificationRequestDto requestDto);
    Task<bool> MarkAsReadAsync(string id, string userId);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DeleteAsync(string id, string userId);
    Task<bool> ClearAllAsync(string userId);
}
