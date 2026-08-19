using Glue.API.Database.Entities.GlueNotification;
using Glue.API.Models.Dtos.Notification;

namespace Glue.API.Mappers;

public static class NotificationMapper
{
    #region -- ToDto()
    /// <summary>
    /// 将 GlueNotification 实体转换为 NotificationDto
    /// </summary>
    public static NotificationDto ToDto(this GlueNotification entity)
    {
        return new NotificationDto
        {
            Id = entity.Id,
            Kind = entity.Kind,
            Level = entity.Level,
            Title = entity.Title,
            Message = entity.Message ?? string.Empty,
            ReleateId = entity.ReleateId,
            CreatedAt = entity.CreatedAt
        };
    }
    #endregion

    #region -- ReuqestToDto
    public static NotificationDto RequestToDto(this NotificationRequestDto requestDto)
    {
        return new NotificationDto
        {
            Kind = requestDto.Kind,
            Level = requestDto.Level,
            Title = requestDto.Title,
            Message = requestDto.Message ?? string.Empty,
            ReleateId = requestDto.ReleateId
        };
    }
    #endregion

    #region -- GlueNotification()
    /// <summary>
    /// 将 NotificationDto 转换为 GlueNotification 实体
    /// </summary>
    public static GlueNotification ToEntity(this NotificationDto dto, string userId)
    {
        return new GlueNotification
        {
            Id = dto.Id,
            UserId = userId,
            Kind = dto.Kind,
            Level = dto.Level,
            Title = dto.Title,
            Message = dto.Message,
            ReleateId = dto.ReleateId
        };
    }
    #endregion

    #region -- ToDtoList()
    /// <summary>
    /// 批量转换实体集合为 DTO 集合
    /// </summary>
    public static IEnumerable<NotificationDto> ToDtoList(this IEnumerable<GlueNotification> entities)
    {
        return entities.Select(entity => ToDto(entity));
    }
    #endregion
}
