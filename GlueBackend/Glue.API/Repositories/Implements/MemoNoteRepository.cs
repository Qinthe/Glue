using Dapper;
using Glue.API.Database;
using Glue.API.Database.Entities.GlueMemoNote;
using Glue.API.Repositories.Interfaces;

namespace Glue.API.Repositories.Implements;

public class MemoNoteRepository : BaseRepository<GlueMemoNote>, IMemoNoteRepository
{
    protected override string TableName => "glue_memo_notes";
    protected override string PrimaryKey => "id";

    public MemoNoteRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    #region -- CreateAsync()
    public override async Task<bool> CreateAsync(GlueMemoNote entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            var sql = @"
                INSERT INTO glue_memo_notes (id, user_id, title, content, category, created_at, updated_at)
                VALUES (@Id, @UserId, @Title, @Content, @Category, @CreatedAt, @UpdatedAt)";

            entity.Id = string.IsNullOrEmpty(entity.Id) ? Guid.NewGuid().ToString("N") : entity.Id;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            await connection.ExecuteAsync(sql, entity, transaction);

            #region -- 插入 Tags
            if (entity.Tags != null && entity.Tags.Any())
            {
                var tagSql = @"
                    INSERT INTO glue_memo_tags (memo_id, tag, created_at, updated_at)
                    VALUES (@MemoId, @Tag, @CreatedAt, @UpdatedAt)";

                foreach (var tag in entity.Tags)
                {
                    tag.CreatedAt = DateTime.UtcNow;
                    tag.UpdatedAt = DateTime.UtcNow;

                    await connection.ExecuteAsync(tagSql, new
                    {
                        MemoId = entity.Id,
                        tag.Tag,
                        tag.CreatedAt,
                        tag.UpdatedAt
                    }, transaction);
                }
            }

            transaction.Commit();
            return true;
            #endregion
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    #endregion

    #region -- UpdateAsync()
    public override async Task<bool> UpdateAsync(GlueMemoNote entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var sql = @"
                UPDATE glue_memo_notes 
                SET title = @Title, 
                    content = @Content, 
                    category = @Category,
                    updated_at = @UpdatedAt
                WHERE id = @Id AND user_id = @UserId";

            entity.UpdatedAt = DateTime.UtcNow;

            var result = await connection.ExecuteAsync(sql, entity, transaction);

            if (result > 0)
            {
                // 删除旧的 Tags
                var deleteSql = "DELETE FROM glue_memo_tags WHERE memo_id = @MemoId";
                await connection.ExecuteAsync(deleteSql, new { MemoId = entity.Id }, transaction);

                // 插入新的 Tags
                if (entity.Tags != null && entity.Tags.Any())
                {
                    var tagSql = @"
                        INSERT INTO glue_memo_tags (memo_id, tag, created_at, updated_at)
                        VALUES (@MemoNoteId, @Tag, @CreatedAt, @UpdatedAt)";

                    foreach (var tag in entity.Tags)
                    {
                        tag.CreatedAt = DateTime.UtcNow;
                        tag.UpdatedAt = DateTime.UtcNow;

                        await connection.ExecuteAsync(tagSql, new
                        {
                            MemoId = entity.Id,
                            tag.Tag,
                            tag.CreatedAt,
                            tag.UpdatedAt
                        }, transaction);
                    }
                }
            }

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    #endregion

    #region -- GetByUserIdAsync()
    public async Task<IEnumerable<GlueMemoNote>>? GetByUserIdAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT  n.*, t.*
            FROM glue_memo_notes n
            LEFT JOIN glue_memo_tags t ON n.id = t.memo_id
            WHERE n.user_id = @UserId
            ORDER BY n.updated_at DESC";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        var noteDictionary = new Dictionary<string, GlueMemoNote>();

        await connection.QueryAsync<GlueMemoNote, GlueMemoTag, GlueMemoNote>(
           sql,
           (note, tag) =>
           {
               if (!noteDictionary.TryGetValue(note.Id, out var currentNote))
               {
                   currentNote = note;
                   currentNote.Tags = new List<GlueMemoTag>();
                   noteDictionary.Add(note.Id, currentNote);
               }

               if (tag?.MemoId != null)
               {
                   ((List<GlueMemoTag>)currentNote.Tags).Add(tag);
               }

               return currentNote;
           },
           parameters,
           splitOn: "memo_id"
        );

        return noteDictionary.Values;
    }
    #endregion

    #region -- SearchByKeywordAsync()
    public async Task<IEnumerable<GlueMemoNote>>? SearchByKeywordAsync(string userId, string keyword)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            SELECT n.*,t.tag
            FROM glue_memo_notes n
            LEFT JOIN glue_memo_tags t ON n.id = t.memo_id
            WHERE n.user_id = @UserId 
            AND (n.title LIKE @Keyword OR n.content LIKE @Keyword or n.category LIKE @Keyword or t.tag LIKE @Keyword)
            ORDER BY n.updated_at DESC;";

        var noteDictionary = new Dictionary<string, GlueMemoNote>();

        await connection.QueryAsync<GlueMemoNote, GlueMemoTag, GlueMemoNote>(
            sql,
            (note, tag) =>
            {
                if (!noteDictionary.TryGetValue(note.Id, out var currentNote))
                {
                    currentNote = note;
                    currentNote.Tags = new List<GlueMemoTag>();
                    noteDictionary.Add(note.Id, currentNote);
                }

                if (tag?.Id != null)
                {
                    ((List<GlueMemoTag>)currentNote.Tags).Add(tag);
                }

                return currentNote;
            },
            new
            {
                UserId = userId,
                Keyword = $"%{keyword}%"
            },
            splitOn: "id"
        );

        return noteDictionary.Values;
    }
    #endregion
}
