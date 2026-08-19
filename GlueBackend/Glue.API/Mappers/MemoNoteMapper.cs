using Glue.API.Database.Entities.GlueMemoNote;
using Glue.API.Models.Dtos.MemoNote;

namespace Glue.API.Mappers;

public static class MemoNoteMapper
{
    #region -- ToDto
    /// <summary>
    /// 将 GlueMemoNote 实体转换为 MemoNoteDto
    /// </summary>
    public static MemoNoteDto ToDto(this GlueMemoNote entity)
    {
        return new MemoNoteDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            Category = entity.Category,
            Tags = entity.Tags?.Select(t => t.Tag) ?? Enumerable.Empty<string>(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
    #endregion

    #region -- RequestToDto
    public static MemoNoteDto RequestToDto(MemoNoteRequestDto request)
    {
        return new MemoNoteDto
        {
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            Tags = request.Tags ?? Enumerable.Empty<string>(),
        };
    }
    #endregion 

    #region -- ToEntity
    /// <summary>
    /// 将 MemoNoteDto 转换为 GlueMemoNote 实体
    /// </summary>
    public static GlueMemoNote ToEntity(this MemoNoteDto dto, string userId)
    {
        return new GlueMemoNote
        {
            Id = dto.Id,
            UserId = userId,
            Title = dto.Title ?? string.Empty,
            Content = dto.Content,
            Category = dto.Category ?? "默认",
            Tags = dto.Tags?.Select(tagName => new GlueMemoTag
            {
                MemoId = dto.Id,
                Tag = tagName
            }).ToList() ?? new List<GlueMemoTag>(),
        };
    }
    #endregion

    #region -- ToDtoList
    /// <summary>
    /// 批量转换实体集合为 DTO 集合
    /// </summary>
    public static IEnumerable<MemoNoteDto> ToDtoList(this IEnumerable<GlueMemoNote> entities)
    {
        return entities.Select(entity => ToDto(entity));
    }
    #endregion
}
