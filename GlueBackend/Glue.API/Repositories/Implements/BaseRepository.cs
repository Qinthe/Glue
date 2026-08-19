using Dapper;
using Glue.API.Database;
using Glue.API.Repositories.Interfaces;

namespace Glue.API.Repositories.Implements;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected abstract string TableName { get; }
    protected abstract string PrimaryKey { get; }

    protected BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region -- GetByIdAsync()
    public virtual async Task<T?> GetByIdAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(
            $"SELECT * FROM {TableName} WHERE {PrimaryKey} = @Id",
            new { Id = id });
    }
    #endregion

    #region -- CreateAsync()
    public abstract Task<bool> CreateAsync(T entity);
    #endregion

    #region -- UpdateAsync()
    public abstract Task<bool> UpdateAsync(T entity);
    #endregion

    #region -- DeleteAsync()
    public virtual async Task<bool> DeleteAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.ExecuteAsync(
            $"DELETE FROM {TableName} WHERE {PrimaryKey} = @Id",
            new { Id = id });
        return result > 0;
    }
    #endregion
}
