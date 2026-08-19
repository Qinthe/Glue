using Glue.API.Models.Dtos.Task;

namespace Glue.API.Services.Interfaces;

public interface ITaskService
{
    Task<TaskWorkbenchDto> GetWorkbenchAsync(string userId);
    Task<IEnumerable<TaskDto>> GetUserTasksAsync(string userId, DateTime? date = null);
    Task<TaskDto?> GetByIdAsync(string id, string userId);
    Task<TaskDto> CreateAsync(string userId, TaskRequestDto requestDto);
    Task<TaskDto?> UpdateAsync(string id, string userId, TaskRequestDto requestDto);
    Task<bool> DeleteAsync(string id, string userId);
    Task<TaskDto?> CompleteTaskAsync(string id, string userId);
    Task<TaskDto?> UpdateReminderAsync(string id, string userId, TaskDto request);
}
