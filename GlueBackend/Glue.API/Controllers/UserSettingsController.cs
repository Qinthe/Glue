using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.Setting;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserSettingsController : BaseController
{
    private readonly IUserSettingsService _service;

    public UserSettingsController(IUserSettingsService service)
    {
        _service = service;
    }

    #region -- Get()
    /// <summary>
    /// 获取用户设置
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<UserSettingDto>>> Get(
        [FromRoute] string userId)
    {
        var currentUserId = GetUserId();

        // 验证用户只能访问自己的设置
        if (userId != currentUserId)
            return Forbid();

        var settings = await _service.GetUserSettingsAsync(userId);

        return Ok(ApiResponseDto<UserSettingDto>.Ok(settings));
    }
    #endregion

    #region -- Update()
    /// <summary>
    /// 更新用户设置（部分更新）
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponseDto<UserSettingDto>>> Update(
        [FromRoute] string userId,
        [FromBody] UserSettingRequestDto request)
    {
        var currentUserId = GetUserId();

        // 验证用户只能更新自己的设置
        if (userId != currentUserId)
            return Forbid();

        var settings = await _service.UpdateSettingsAsync(userId, request);

        return Ok(ApiResponseDto<UserSettingDto>.Ok(settings, "设置更新成功"));
    }
    #endregion

    #region -- Reset()
    /// <summary>
    /// 重置用户设置为默认值
    /// </summary>
    [HttpPost("reset")]
    public async Task<ActionResult<ApiResponseDto<UserSettingDto>>> Reset(
        [FromRoute] string userId)
    {
        var currentUserId = GetUserId();

        // 验证用户只能重置自己的设置
        if (userId != currentUserId)
            return Forbid();

        var settings = await _service.ResetSettingsAsync(userId);

        return Ok(ApiResponseDto<UserSettingDto>.Ok(settings, "设置已重置为默认值"));
    }
    #endregion
}
