using Glue.API.Database.Entities.GlueTask;
using Glue.API.Models.Dtos.Task;

namespace Glue.API.Mappers;

public static class TaskMapper
{
    #region -- ToDto()
    /// <summary>
    /// 将 GlueTask 实体转换为 TaskDto
    /// </summary>
    public static TaskDto ToDto(this GlueTask entity)
    {
        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            ScheduledDate = entity.ScheduledDate,
            StartAt = entity.StartAt,
            EndAt = entity.EndAt,
            Progress = entity.Progress,
            Status = entity.Status,
            ReminderEnabled = entity.ReminderEnabled,
            ReminderMinutesBefore = entity.ReminderMinutesBefore,
            LastReminderAt = entity.LastReminderAt,
            CompletedAt = entity.CompletedAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    #endregion

    #region -- RequestToDto()
    public static TaskDto RequestToDto(this TaskRequestDto request)
    {
        return new TaskDto
        {
            Title = request.Title,
            Description = request.Description,
            ScheduledDate = request.ScheduledDate,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Progress = request.Progress,
            Status = request.Status,
            ReminderEnabled = request.ReminderEnabled,
            ReminderMinutesBefore = request.ReminderMinutesBefore,
            LastReminderAt = request.LastReminderAt,
            CompletedAt = request.CompletedAt
        };
    }
    #endregion

    #region -- ToEntity()
    /// <summary>
    /// 将 TaskDto 转换为 GlueTask 实体
    /// </summary>
    public static GlueTask ToEntity(this TaskDto request, string userId)
    {
        return new GlueTask
        {
            Id = string.Empty,
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            ScheduledDate = request.ScheduledDate,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Progress = request.Progress,
            Status = request.Status,
            ReminderEnabled = request.ReminderEnabled,
            ReminderMinutesBefore = request.ReminderMinutesBefore
        };
    }
    #endregion

    #region -- ApplyUpdate()
    /// <summary>
    /// 将 TaskDto 应用到现有实体（更新）
    /// </summary>
    public static void ApplyUpdate(this GlueTask entity, TaskDto request)
    {
        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ScheduledDate = request.ScheduledDate;
        entity.StartAt = request.StartAt;
        entity.EndAt = request.EndAt;
        entity.Progress = request.Progress;
        entity.Status = request.Status;
        entity.ReminderEnabled = request.ReminderEnabled;
        entity.ReminderMinutesBefore = request.ReminderMinutesBefore;
    }
    #endregion

    #region -- ToDtoList()
    /// <summary>
    /// 批量转换实体集合为 DTO 集合
    /// </summary>
    public static IEnumerable<TaskDto> ToDtoList(this IEnumerable<GlueTask> entities)
    {
        return entities.Select(entity => ToDto(entity));
    }
    #endregion

    #region -- ToWorkbenchGroup()
    /// <summary>
    /// 创建任务分组
    /// </summary>
    public static TaskWorkbenchDto ToWorkbenchGroup(string groupId, string groupName, IEnumerable<GlueTask> tasks)
    {
        return new TaskWorkbenchDto
        {
            GroupId = groupId,
            GroupName = groupName,
            Tasks = tasks.ToDtoList()
        };
    }
    #endregion
}
