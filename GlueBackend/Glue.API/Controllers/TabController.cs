using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.Tab;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glue.API.Controllers;

[ApiController]
[Route("api/tab")]
public class TabController : BaseController
{
    private readonly ITabService _service;

    public TabController(ITabService service)
    {
        _service = service;
    }

    #region -- GetAll()
    /// <summary>
    /// 获取用户所有标签页
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<TabDto>>>> GetAll(
        [FromRoute] string userId)
    {
        var currentUserId = GetUserId();

        // 验证用户只能访问自己的标签页
        if (userId != currentUserId)
            return Forbid();

        var tabs = await _service.GetUserTabsAsync(userId);

        // 按 SortOrder 和创建时间排序
        tabs = tabs.OrderBy(t => t.SortOrder).ThenBy(t => t.CreatedAt);

        return Ok(ApiResponseDto<IEnumerable<TabDto>>.Ok(tabs));
    }
    #endregion

    #region -- Create()
    /// <summary>
    /// 创建新标签页
    /// </summary>
    [HttpPost("{userId}")]
    public async Task<ActionResult<ApiResponseDto<TabDto>>> Create(
        [FromRoute] string userId,
        [FromBody] TabRequestDto request)
    {
        var currentUserId = GetUserId();

        // 验证用户只能创建自己的标签页
        if (userId != currentUserId)
            return Forbid();

        var tab = await _service.CreateAsync(userId, request);

        return Ok(ApiResponseDto<TabDto>.Ok(tab, "标签页创建成功"));
    }
    #endregion

    #region -- Update()
    /// <summary>
    /// 更新标签页
    /// </summary>
    [HttpPut("{userId}/{id}")]
    public async Task<ActionResult<ApiResponseDto<TabDto>>> Update(
        [FromRoute] string userId,
        [FromRoute] string id,
        [FromBody] TabRequestDto request)
    {
        var currentUserId = GetUserId();

        // 验证用户只能更新自己的标签页
        if (userId != currentUserId)
            return Forbid();

        var tab = await _service.UpdateAsync(id, userId, request);

        if (tab == null)
            return BadRequest(ApiResponseDto<TabDto>.Error("更新失败，标签页不存在或无权操作"));

        return Ok(ApiResponseDto<TabDto>.Ok(tab, "标签页更新成功"));
    }
    #endregion

    #region -- Delete()
    /// <summary>
    /// 删除标签页
    /// </summary>
    [HttpDelete("{userId}/{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(
        [FromRoute] string userId,
        [FromRoute] string id)
    {
        var currentUserId = GetUserId();

        // 验证用户只能删除自己的标签页
        if (userId != currentUserId)
            return Forbid();

        var result = await _service.DeleteAsync(id, userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("删除失败，标签页不存在或无权操作"));

        return Ok(ApiResponseDto<object>.Ok(null, "标签页已删除"));
    }
    #endregion

    #region -- Reorder()
    /// <summary>
    /// 重新排序标签页
    /// </summary>
    [HttpPut("reorder")]
    public async Task<ActionResult<ApiResponseDto<object>>> Reorder(
        [FromRoute] string userId,
        [FromBody] IEnumerable<TabOrderItemDto> Items)
    {
        var currentUserId = GetUserId();

        // 验证用户只能重新排序自己的标签页
        if (userId != currentUserId)
            return Forbid();

        var result = await _service.ReorderTabsAsync(userId, Items);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("重新排序失败"));

        return Ok(ApiResponseDto<object>.Ok(null, "标签页已重新排序"));
    }
    #endregion
}
