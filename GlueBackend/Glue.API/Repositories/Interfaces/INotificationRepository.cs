using Glue.API.Database.Entities.GlueNotification;

namespace Glue.API.Repositories.Interfaces;

public interface INotificationRepository : IBaseRepository<GlueNotification>
{
    Task<IEnumerable<GlueNotification>>? GetByUserIdAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task<bool> MarkAsReadAsync(string id);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<bool> DeleteOldNotificationsAsync(string userId, DateTime beforeDate);
}
