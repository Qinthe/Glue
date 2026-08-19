using Glue.API.Mappers;
using Glue.API.Models.Dtos.Task;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services.Interfaces;

namespace Glue.API.Services.Implements;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    #region -- GetWorkbenchAsync()
    /// <summary>
    /// 获取任务工作台数据（返回今天的任务分组）
    /// 如果需要其他分组，可以调整返回逻辑
    /// </summary>
    public async Task<TaskWorkbenchDto> GetWorkbenchAsync(string userId)
    {
        // 获取用户的所有任务
        var allTasks = (await _repository.GetByUserIdAsync(userId)).ToList();
        var today = DateTime.UtcNow.Date;

        // 今天的任务（未完成）
        var todayTasks = allTasks
            .Where(t => t.ScheduledDate == today && t.Status != "completed")
            .OrderBy(t => t.StartAt)
            .ToList();

        // 返回今天的任务分组
        // 如果你需要返回多个分组，考虑修改返回类型为 IEnumerable<TaskWorkbenchDto>
        return TaskMapper.ToWorkbenchGroup("today", "今天", todayTasks);
    }
    #endregion

    #region -- GetUserTasksAsync()
    public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(string userId, DateTime? date = null)
    {
        var tasks = await _repository.GetByUserIdAsync(userId, date);
        return tasks.ToDtoList();
    }
    #endregion

    #region -- GetByIdAsync()
    public async Task<TaskDto?> GetByIdAsync(string id, string userId)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null || task.UserId != userId)
            return null;

        return task.ToDto();
    }
    #endregion

    #region -- CreateAsync()
    public async Task<TaskDto> CreateAsync(string userId, TaskRequestDto requestDto)
    {
        var dto = TaskMapper.RequestToDto(requestDto);
        var entity = TaskMapper.ToEntity(dto,userId);
        await _repository.CreateAsync(entity);
        return entity.ToDto();
    }
    #endregion

    #region -- UpdateAsync()
    public async Task<TaskDto?> UpdateAsync(string id, string userId, TaskRequestDto requestDto)
    {
        var dto = TaskMapper.RequestToDto(requestDto);
        var task = await _repository.GetByIdAsync(id);

        if (task == null || task.UserId != userId)
            return null;

        TaskMapper.ApplyUpdate(task, dto);

        var updated = await _repository.UpdateAsync(task);
        return updated ? task.ToDto() : null;
    }
    #endregion

    #region -- DeleteAsync()
    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null || task.UserId != userId)
            return false;

        return await _repository.DeleteAsync(id);
    }
    #endregion

    #region -- CompleteTaskAsync()
    public async Task<TaskDto?> CompleteTaskAsync(string id, string userId)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null || task.UserId != userId)
            return null;

        task.Status = "completed";
        task.Progress = 100;
        task.CompletedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(task);
        return updated ? task.ToDto() : null;
    }
    #endregion

    #region -- UpdateReminderAsync()
    public async Task<TaskDto?> UpdateReminderAsync(string id, string userId, TaskDto request)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null || task.UserId != userId)
            return null;

        task.ReminderEnabled = request.ReminderEnabled;
        task.ReminderMinutesBefore = request.ReminderMinutesBefore;

        var updated = await _repository.UpdateAsync(task);
        return updated ? task.ToDto() : null;
    }
    #endregion
}
