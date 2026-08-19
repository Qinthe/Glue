using Glue.API.Mappers;
using Glue.API.Models.Dtos.Notification;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services.Interfaces;

namespace Glue.API.Services.Implements;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;

    public NotificationService(INotificationRepository repository)
    {
        _repository = repository;
    }

    #region -- GetUserNotificationsAsync()
    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool? onlyUnread = null, string? kind = null)
    {
        var notifications = await _repository.GetByUserIdAsync(userId);

        // 过滤未读通知
        if (onlyUnread == true)
        {
            notifications = notifications.Where(n => n.ReadAt == null);
        }

        // 按类型过滤
        if (!string.IsNullOrEmpty(kind))
        {
            notifications = notifications.Where(n => n.Kind == kind);
        }

        return notifications.ToDtoList();
    }
    #endregion

    #region -- GetUnreadCountAsync()
    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _repository.GetUnreadCountAsync(userId);
    }
    #endregion

    #region -- CreateAsync()
    public async Task<NotificationDto> CreateAsync(string userId, NotificationRequestDto requestDto)
    {
        var dto = NotificationMapper.RequestToDto(requestDto);

        var entity = NotificationMapper.ToEntity(dto,userId);
        await _repository.CreateAsync(entity);
        return entity.ToDto();
    }
    #endregion

    #region -- UpsertAsync()
    public async Task<NotificationDto?> UpsertAsync(string id, string userId, NotificationRequestDto requestDto)
    {
        var existingNotification = await _repository.GetByIdAsync(id);

        var dto = NotificationMapper.RequestToDto(requestDto);

        if (existingNotification == null)
        {
            
            var entity = NotificationMapper.ToEntity(dto,userId);
            entity.Id = id;
            await _repository.CreateAsync(entity);
            return entity.ToDto();
        }
        else
        {
            // 验证权限
            if (existingNotification.UserId != userId)
                return null;

            // 更新通知
            var entity = dto.ToEntity(userId);
            entity.Id = id;
            entity.CreatedAt = existingNotification.CreatedAt;
            entity.ReadAt = existingNotification.ReadAt;

            var updated = await _repository.UpdateAsync(entity);
            return updated ? entity.ToDto() : null;
        }
    }
    #endregion

    #region -- MarkAsReadAsync()
    public async Task<bool> MarkAsReadAsync(string id, string userId)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null || notification.UserId != userId)
            return false;

        return await _repository.MarkAsReadAsync(id);
    }
    #endregion

    #region -- MarkAllAsReadAsync()
    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        return await _repository.MarkAllAsReadAsync(userId);
    }
    #endregion

    #region -- DeleteAsync()
    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null || notification.UserId != userId)
            return false;

        return await _repository.DeleteAsync(id);
    }
    #endregion

    #region -- ClearAllAsync()
    public async Task<bool> ClearAllAsync(string userId)
    {
        var notifications = await _repository.GetByUserIdAsync(userId);

        foreach (var notification in notifications)
        {
            await _repository.DeleteAsync(notification.Id);
        }

        return true;
    }
    #endregion
}