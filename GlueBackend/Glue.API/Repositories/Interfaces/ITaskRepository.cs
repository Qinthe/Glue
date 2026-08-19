using Glue.API.Database.Entities.GlueTask;

namespace Glue.API.Repositories.Interfaces;

public interface ITaskRepository : IBaseRepository<GlueTask>
{
    Task<IEnumerable<GlueTask>>? GetByUserIdAsync(string userId, DateTime? date = null);
    Task<IEnumerable<GlueTask>>? GetByGroupAsync(string userId, string group);
    Task<IEnumerable<GlueTaskGroup>>? GetGroupByUserID(string userId);
    Task<IEnumerable<GlueTask>>? GetTasksNeedingReminderAsync(DateTime currentTime);
    Task<bool> UpdateStatusAsync(string id, string status);
    Task<bool> UpdateProgressAsync(string id, int progress);
    Task<bool> UpdateReminderAsync(string id, DateTime lastReminderAt);
    Task<bool> CompleteTaskAsync(string id);
}
