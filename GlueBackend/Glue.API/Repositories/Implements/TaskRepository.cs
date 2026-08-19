using Dapper;
using Glue.API.Database;
using Glue.API.Database.Entities.GlueTask;
using Glue.API.Repositories.Interfaces;

namespace Glue.API.Repositories.Implements;

public class TaskRepository : BaseRepository<GlueTask>, ITaskRepository
{
    protected override string TableName => "glue_tasks";
    protected override string PrimaryKey => "id";

    public TaskRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    #region -- CreateAsync()
    public override async Task<bool> CreateAsync(GlueTask entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO glue_tasks (id, user_id, title, description, scheduled_date, start_at, end_at, progress, status, 
                reminder_enabled, reminder_minutes_before, last_reminder_at, completed_at, 
                created_at, updated_at)
            VALUES (@Id, @UserId, @Title, @Description, @ScheduledDate, @StartAt, @EndAt, @Progress, @Status, 
                    @ReminderEnabled, @ReminderMinutesBefore, @LastReminderAt, @CompletedAt, @CreatedAt, @UpdatedAt)";

        entity.Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid().ToString("N") : entity.Id;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await connection.ExecuteAsync(sql, entity);
        return true;
    }
    #endregion

    #region -- UpdateAsync()
    public override async Task<bool> UpdateAsync(GlueTask entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tasks 
                SET title = @Title, 
                    description = @Description,
                    scheduled_date = @ScheduledDate,
                    start_at = @StartAt,
                    end_at = @EndAt,
                    progress = @Progress,
                    status = @Status,
                    reminder_enabled = @ReminderEnabled,
                    reminder_minutes_before = @ReminderMinutesBefore,
                    last_reminder_at = @LastReminderAt,
                    completed_at = @CompletedAt,
                    updated_at = @UpdatedAt
                WHERE id = @Id AND user_id = @UserId";

        entity.UpdatedAt = DateTime.UtcNow;

        var result = await connection.ExecuteAsync(sql, entity);
        return result > 0;
    }
    #endregion

    #region -- GetByUserIdAsync()
    public async Task<IEnumerable<GlueTask>> GetByUserIdAsync(string userId, DateTime? date = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT * 
            FROM glue_tasks 
            WHERE user_id = @UserId";
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        if (date.HasValue)
        {
            sql += " AND scheduled_date = @ScheduledDate";
            parameters.Add("ScheduledDate", date.Value.Date);
        }

        sql += " ORDER BY start_at ASC";

        return await connection.QueryAsync<GlueTask>(sql, parameters);
    }
    #endregion

    #region -- GetByGroupAsync()
    public async Task<IEnumerable<GlueTask>> GetByGroupAsync(string userId, string group)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT * 
            FROM glue_tasks t
            LEFT JOIN glue_task_groups g ON t.id = g.task_id 
            WHERE t.user_id = @UserId AND g.group_name = @Group
            ORDER BY t.scheduled_date DESC, t.start_at ASC";

        return await connection.QueryAsync<GlueTask>(sql, new
        {
            UserId = userId,
            Group = group
        });
    }
    #endregion

    #region -- GetGroupByUserID()
    public async Task<IEnumerable<GlueTaskGroup>> GetGroupByUserID(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT * 
            FROM glue_tasks t
            LEFT JOIN glue_task_groups g ON t.id = g.task_id 
            WHERE t.user_id = @UserId";

        return await connection.QueryAsync<GlueTaskGroup>(sql, new
        {
            UserId = userId
        });
    }
    #endregion

    #region -- GetTasksNeedingReminderAsync()
    public async Task<IEnumerable<GlueTask>> GetTasksNeedingReminderAsync(DateTime currentTime)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                SELECT * FROM glue_tasks 
                WHERE reminder_enabled = 1 
                AND status = 'pending'
                AND (last_reminder_at IS NULL OR last_reminder_at < @ReminderThreshold)
                AND start_at <= @CurrentTime
                ORDER BY start_at ASC";

        var reminderThreshold = currentTime.AddMinutes(-30); // 30分钟内不再重复提醒

        return await connection.QueryAsync<GlueTask>(sql, new
        {
            CurrentTime = currentTime,
            ReminderThreshold = reminderThreshold
        });
    }
    #endregion

    #region -- UpdateStatusAsync()
    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tasks 
                SET status = @Status, updated_at = @UpdatedAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Status = status,
            UpdatedAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- UpdateProgressAsync()
    public async Task<bool> UpdateProgressAsync(string id, int progress)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tasks 
                SET progress = @Progress, updated_at = @UpdatedAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Progress = progress,
            UpdatedAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- UpdateReminderAsync()
    public async Task<bool> UpdateReminderAsync(string id, DateTime lastReminderAt)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tasks 
                SET last_reminder_at = @LastReminderAt, updated_at = @UpdatedAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            LastReminderAt = lastReminderAt,
            UpdatedAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- CompleteTaskAsync()
    public async Task<bool> CompleteTaskAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tasks 
                SET status = 'completed', 
                    progress = 100, 
                    completed_at = @CompletedAt,
                    updated_at = @UpdatedAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            CompletedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion
}
