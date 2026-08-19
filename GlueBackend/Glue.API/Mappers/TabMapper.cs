using Glue.API.Database.Entities.GlueTab;
using Glue.API.Models.Dtos.Tab;

namespace Glue.API.Mappers;

public static class TabMapper
{
    #region -- ToDto()
    /// <summary>
    /// 将 GlueTab 实体转换为 TabDto
    /// </summary>
    public static TabDto ToDto(this GlueTab entity)
    {
        return new TabDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Url = entity.Url,
            Icon = entity.Icon,
            Image = entity.Image,
            Category = entity.Category,
            OpenMode = entity.OpenMode,
            SortOrder = entity.SortOrder,
            IsPinned = entity.IsPinned,
            Description = entity.Description,
            Color = entity.Color,
            CreatedAt = entity.CreatedAt
        };
    }
    #endregion

    #region -- ToEntity()
    /// <summary>
    /// 将 TabDto 转换为 GlueTab 实体
    /// </summary>
    public static GlueTab ToEntity(this TabDto request, string userId)
    {
        return new GlueTab
        {
            Id = request.Id,
            UserId = userId,
            Title = request.Title,
            Url = request.Url,
            Icon = request.Icon,
            Image = request.Image,
            Category = request.Category,
            OpenMode = request.OpenMode,
            SortOrder = request.SortOrder,
            IsPinned = request.IsPinned,
            Description = request.Description,
            Color = request.Color
        };
    }
    #endregion

    #region -- RequestToDt
    /// <summary>
    /// 将 TabRequestDto 转换为 TabDto 实体
    /// </summary>
    public static TabDto RequestToDt(this TabRequestDto request)
    {
        return new TabDto
        {
            Title = request.Title,
            Url = request.Url,
            Icon = request.Icon,
            Image = request.Image,
            Category = request.Category,
            OpenMode = request.OpenMode,
            SortOrder = request.SortOrder,
            IsPinned = request.IsPinned,
            Description = request.Description,
            Color = request.Color
        };
    }
    #endregion

    #region -- ApplyUpdate()
    /// <summary>
    /// 将 UpdatePortalTabRequest 应用到现有实体（部分更新）
    /// </summary>
    public static void ApplyUpdate(this GlueTab entity, TabDto request)
    {
        if (request.Title != null) entity.Title = request.Title;
        if (request.Url != null) entity.Url = request.Url;
        if (request.Icon != null) entity.Icon = request.Icon;
        if (request.Image != null) entity.Image = request.Image;
        if (request.Category != null) entity.Category = request.Category;
        entity.OpenMode = request.OpenMode;
        if (request.SortOrder != 0) entity.SortOrder = request.SortOrder;
        if (request.IsPinned) entity.IsPinned = request.IsPinned;
        if (request.Description != null) entity.Description = request.Description;
        if (request.Color != null) entity.Color = request.Color;
    }
    #endregion

    #region -- ToDtoList()
    /// <summary>
    /// 批量转换实体集合为 DTO 集合
    /// </summary>
    public static IEnumerable<TabDto> ToDtoList(this IEnumerable<GlueTab> entities)
    {
        return entities.Select(entity => ToDto(entity));
    }
    #endregion
}
