using Glue.API.Models.Dtos.Tab;

namespace Glue.API.Services.Interfaces;

public interface ITabService
{
    Task<IEnumerable<TabDto>> GetUserTabsAsync(string userId);
    Task<IEnumerable<TabDto>> GetByCategoryAsync(string userId, string category);
    Task<TabDto?> GetByIdAsync(string id, string userId);
    Task<TabDto> CreateAsync(string userId, TabRequestDto requestDto);
    Task<TabDto?> UpdateAsync(string id, string userId, TabRequestDto requestDto);
    Task<bool> DeleteAsync(string id, string userId);
    Task<bool> ReorderTabsAsync(string userId, IEnumerable<TabOrderItemDto> items);
}
