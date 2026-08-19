using Glue.API.Database.Entities.GlueTab;

namespace Glue.API.Repositories.Interfaces;

public interface ITabRepository : IBaseRepository<GlueTab>
{
    Task<IEnumerable<GlueTab>>? GetByUserIdAsync(string userId);
    Task<IEnumerable<GlueTab>>? GetByCategoryAsync(string userId, string category);
    Task<bool> UpdateSortOrderAsync(string id, int sortOrder);
    Task<bool> TogglePinAsync(string id, bool isPinned);
    Task<bool> BatchUpdateSortOrderAsync(List<(string Id, int SortOrder)> items);
}
