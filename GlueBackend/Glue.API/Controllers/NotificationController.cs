using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.Notification;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : BaseController
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    #region -- GetList()
    /// <summary>
    /// 获取用户通知列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<NotificationDto>>>> GetList(
        [FromRoute] string userId,
        [FromQuery] bool? onlyUnread,
        [FromQuery] string? kind,
        [FromQuery] int pageSize = 100)
    {
        var currentUserId = GetUserId();

        // 验证用户只能访问自己的通知
        if (userId != currentUserId)
            return Forbid();

        var notifications = await _service.GetUserNotificationsAsync(userId, onlyUnread, kind);

        // 限制返回数量
        notifications = notifications.Take(pageSize);

        return Ok(ApiResponseDto<IEnumerable<NotificationDto>>.Ok(notifications));
    }
    #endregion

    #region -- Create()
    /// <summary>
    /// 创建通知
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<NotificationDto>>> Create(
        [FromRoute] string userId,
        [FromBody] NotificationRequestDto request)
    {
        var currentUserId = GetUserId();

        // 验证用户只能创建自己的通知
        if (userId != currentUserId)
            return Forbid();

        var notification = await _service.CreateAsync(userId, request);

        return Ok(ApiResponseDto<NotificationDto>.Ok(notification, "通知创建成功"));
    }
    #endregion

    #region -- Upsert()
    /// <summary>
    /// 创建或更新通知
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<NotificationDto>>> Upsert(
        [FromRoute] string userId,
        [FromRoute] string id,
        [FromBody] NotificationRequestDto request)
    {
        var currentUserId = GetUserId();

        // 验证用户只能操作自己的通知
        if (userId != currentUserId)
            return Forbid();

        var notification = await _service.UpsertAsync(id, userId, request);

        if (notification == null)
            return BadRequest(ApiResponseDto<NotificationDto>.Error("操作失败，通知不存在或无权操作"));

        return Ok(ApiResponseDto<NotificationDto>.Ok(notification, "操作成功"));
    }
    #endregion

    #region -- MarkRead()
    /// <summary>
    /// 标记通知为已读
    /// </summary>
    [HttpPatch("{id}/read")]
    public async Task<ActionResult<ApiResponseDto<object>>> MarkRead(
        [FromRoute] string userId,
        [FromRoute] string id)
    {
        var currentUserId = GetUserId();

        // 验证用户只能操作自己的通知
        if (userId != currentUserId)
            return Forbid();

        var result = await _service.MarkAsReadAsync(id, userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("标记失败，通知不存在或无权操作"));

        return Ok(ApiResponseDto<object>.Ok(null, "已标记为已读"));
    }
    #endregion

    #region -- MarkAllRead()
    /// <summary>
    /// 标记所有通知为已读
    /// </summary>
    [HttpPatch("read-all")]
    public async Task<ActionResult<ApiResponseDto<object>>> MarkAllRead(
        [FromRoute] string userId)
    {
        var currentUserId = GetUserId();

        // 验证用户只能操作自己的通知
        if (userId != currentUserId)
            return Forbid();

        var result = await _service.MarkAllAsReadAsync(userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("操作失败"));

        return Ok(ApiResponseDto<object>.Ok(null, "所有通知已标记为已读"));
    }
    #endregion

    #region -- Delete()
    /// <summary>
    /// 删除单个通知
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> Delete(
        [FromRoute] string userId,
        [FromRoute] string id)
    {
        var currentUserId = GetUserId();

        // 验证用户只能删除自己的通知
        if (userId != currentUserId)
            return Forbid();

        var result = await _service.DeleteAsync(id, userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("删除失败，通知不存在或无权操作"));

        return Ok(ApiResponseDto<object>.Ok(null, "通知已删除"));
    }
    #endregion

    #region -- ClearAll()
    /// <summary>
    /// 清空所有通知
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponseDto<object>>> ClearAll(
        [FromRoute] string userId)
    {
        var currentUserId = GetUserId();

        // 验证用户只能清空自己的通知
        if (userId != currentUserId)
            return Forbid();

        var result = await _service.ClearAllAsync(userId);

        if (!result)
            return BadRequest(ApiResponseDto<object>.Error("清空失败"));

        return Ok(ApiResponseDto<object>.Ok(null, "所有通知已清空"));
    }
    #endregion
}
