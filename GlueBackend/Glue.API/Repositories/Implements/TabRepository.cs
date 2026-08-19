using Dapper;
using Glue.API.Database;
using Glue.API.Database.Entities.GlueTab;
using Glue.API.Repositories.Interfaces;

namespace Glue.API.Repositories.Implements;

public class TabRepository : BaseRepository<GlueTab>, ITabRepository
{
    protected override string TableName => "glue_tabs";
    protected override string PrimaryKey => "id";

    public TabRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    #region -- CreateAsync()
    public override async Task<bool> CreateAsync(GlueTab entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                INSERT INTO glue_tabs (id, user_id, title, url, icon, image, category, open_mode, sort_order, is_pinned, description, color, created_at, updated_at)
                VALUES (@Id, @UserId, @Title, @Url, @Icon, @Image, @Category, @OpenMode, @SortOrder, @IsPinned, @Description, @Color, @CreatedAt, @UpdatedAt)";

        entity.Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid().ToString("N") : entity.Id;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await connection.ExecuteAsync(sql, entity);
        return true;
    }

    #endregion

    #region -- UpdateAsync()
    public override async Task<bool> UpdateAsync(GlueTab entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tabs 
                SET title = @Title, 
                    url = @Url, 
                    icon = @Icon,
                    image = @Image,
                    category = @Category,
                    open_mode = @OpenMode,
                    sort_order = @SortOrder,
                    is_pinned = @IsPinned,
                    description = @Description,
                    color = @Color,
                    updated_at = @UpdatedAt
                WHERE id = @Id AND user_id = @UserId";

        entity.UpdatedAt = DateTime.UtcNow;

        var result = await connection.ExecuteAsync(sql, entity);
        return result > 0;
    }

    #endregion

    #region -- GetByUserIdAsync()
    public async Task<IEnumerable<GlueTab>> GetByUserIdAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                SELECT * FROM glue_tabs 
                WHERE user_id = @UserId 
                ORDER BY is_pinned DESC, sort_order ASC, updated_at DESC";

        return await connection.QueryAsync<GlueTab>(sql, new { UserId = userId });
    }

    #endregion

    #region -- GetByCategoryAsync()
    public async Task<IEnumerable<GlueTab>> GetByCategoryAsync(string userId, string category)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                SELECT * FROM glue_tabs 
                WHERE user_id = @UserId AND category = @Category 
                ORDER BY is_pinned DESC, sort_order ASC, updated_at DESC";

        return await connection.QueryAsync<GlueTab>(sql, new
        {
            UserId = userId,
            Category = category
        });
    }
    #endregion

    #region -- UpdateSortOrderAsync()
    public async Task<bool> UpdateSortOrderAsync(string id, int sortOrder)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tabs 
                SET sort_order = @SortOrder, updated_at = @UpdatedAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            SortOrder = sortOrder,
            UpdatedAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- TogglePinAsync()
    public async Task<bool> TogglePinAsync(string id, bool isPinned)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
                UPDATE glue_tabs 
                SET is_pinned = @IsPinned, updated_at = @UpdatedAt
                WHERE id = @Id";

        var result = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            IsPinned = isPinned,
            UpdatedAt = DateTime.UtcNow
        });

        return result > 0;
    }
    #endregion

    #region -- BatchUpdateSortOrderAsync()
    public async Task<bool> BatchUpdateSortOrderAsync(List<(string Id, int SortOrder)> items)
    {
        using var connection = _connectionFactory.CreateConnection();

        using var transaction = connection.BeginTransaction();
        try
        {
            var sql = @"
                    UPDATE glue_tabs 
                    SET sort_order = @SortOrder, updated_at = @UpdatedAt
                    WHERE id = @Id";

            foreach (var (id, sortOrder) in items)
            {
                await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    SortOrder = sortOrder,
                    UpdatedAt = DateTime.UtcNow
                }, transaction);
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    #endregion
}
