using Glue.API.Mappers;
using Glue.API.Models.Dtos.Tab;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services.Interfaces;

namespace Glue.API.Services.Implements;

public class TabService : ITabService
{
    private readonly ITabRepository _repository;

    public TabService(ITabRepository repository)
    {
        _repository = repository;
    }

    #region -- GetUserTabsAsync()
    public async Task<IEnumerable<TabDto>> GetUserTabsAsync(string userId)
    {
        var tabs = await _repository.GetByUserIdAsync(userId);
        return tabs.ToDtoList();
    }
    #endregion

    #region -- GetByCategoryAsync()
    public async Task<IEnumerable<TabDto>> GetByCategoryAsync(string userId, string category)
    {
        var tabs = await _repository.GetByCategoryAsync(userId, category);
        return tabs.ToDtoList();
    }
    #endregion

    #region -- GetByIdAsync()
    public async Task<TabDto?> GetByIdAsync(string id, string userId)
    {
        var tab = await _repository.GetByIdAsync(id);

        if (tab == null || tab.UserId != userId)
            return null;

        return tab.ToDto();
    }
    #endregion

    #region -- CreateAsync()
    public async Task<TabDto> CreateAsync(string userId, TabRequestDto requestDto)
    {
        var dto = TabMapper.RequestToDt(requestDto);
        var entity = TabMapper.ToEntity(dto,userId);
        await _repository.CreateAsync(entity);
        return entity.ToDto();
    }
    #endregion

    #region -- UpdateAsync()
    public async Task<TabDto?> UpdateAsync(string id, string userId, TabRequestDto requestDto)
    {
        var tab = await _repository.GetByIdAsync(id);

        if (tab == null || tab.UserId != userId)
            return null;

        var dto = TabMapper.RequestToDt(requestDto);
        tab.ApplyUpdate(dto);

        var updated = await _repository.UpdateAsync(tab);

        return updated ? tab.ToDto() : null;
    }
    #endregion

    #region -- DeleteAsync()
    public async Task<bool> DeleteAsync(string id, string userId)
    {
        var tab = await _repository.GetByIdAsync(id);

        if (tab == null || tab.UserId != userId)
            return false;

        return await _repository.DeleteAsync(id);
    }
    #endregion

    #region -- ReorderTabsAsync()
    public async Task<bool> ReorderTabsAsync(string userId, IEnumerable<TabOrderItemDto> items)
    {
        foreach (var item in items)
        {
            var tab = await _repository.GetByIdAsync(item.Id);

            // 验证标签属于该用户
            if (tab == null || tab.UserId != userId)
                continue;

            tab.SortOrder = item.SortOrder;
            await _repository.UpdateAsync(tab);
        }

        return true;
    }
    #endregion
}
