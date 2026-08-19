using Dapper;
using Glue.API.Database;
using Glue.API.Database.Entities.GlueNotification;
using Glue.API.Repositories.Interfaces;

namespace Glue.API.Repositories.Implements;

public class NotificationRepository : BaseRepository<GlueNotification>, INotificationRepository
{
    protected override string TableName => "glue_notifications";
    protected override string PrimaryKey => "id";

    public NotificationRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    #region -- CreateAsync()
    public override async Task<bool> CreateAsync(GlueNotification entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                INSERT INTO glue_notifications (id, user_id, kind, level, title, message, releate_id, created_at, read_at)
                VALUES (@Id, @UserId, @Kind, @Level, @Title, @Message, @ReleateId, @CreatedAt, @ReadAt)";

        entity.Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid().ToString("N") : entity.Id;
        entity.CreatedAt = DateTime.UtcNow;

        await connection.ExecuteAsync(sql, entity);
        return true;
    }
    #endregion

    #region -- GetByUserIdAsync()
    public async Task<IEnumerable<GlueNotification>>? GetByUserIdAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT * 
            FROM glue_notifications 
            WHERE user_id = @UserId
            ORDER BY created_at DESC;";

        return await connection.QueryAsync<GlueNotification>(sql, new
        {
            UserId = userId
        });
    }
    #endregion

    #region -- GetUnreadCountAsync()
    public async Task<int> GetUnreadCountAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                SELECT COUNT(1) FROM glue_notifications 
                WHERE user_id = @UserId AND read_at IS NULL";

        return await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId });
    }
    #endregion

    #region -- MarkAsReadAsync()
    public async Task<bool> MarkAsReadAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_notifications 
                SET read_at = @ReadAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            ReadAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- MarkAllAsReadAsync()
    public async Task<bool> MarkAllAsReadAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_notifications 
                SET read_at = @ReadAt
                WHERE user_id = @UserId AND read_at IS NULL";

        var result = await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            ReadAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- DeleteOldNotificationsAsync()
    public async Task<bool> DeleteOldNotificationsAsync(string userId, DateTime beforeDate)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                DELETE FROM glue_notifications 
                WHERE user_id = @UserId AND created_at < @BeforeDate";

        var result = await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            BeforeDate = beforeDate
        });

        return result > 0;
    }
    #endregion

    #region -- UpdateAsync()
    public override Task<bool> UpdateAsync(GlueNotification entity)
    {
        throw new NotImplementedException();
    }
    #endregion
}
