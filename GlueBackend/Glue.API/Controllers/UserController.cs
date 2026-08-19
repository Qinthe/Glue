using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.User;
using Glue.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Glue.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #region -- Register()
    /// <summary>
    /// 用户注册
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _userService.RegisterAsync(request, ipAddress);
        return StatusCode(result.StatusCode, result);
    }

    #endregion

    #region -- Login()
    /// <summary>
    /// 用户登录
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _userService.LoginAsync(request, ipAddress);
        return StatusCode(result.StatusCode, result);
    }
    #endregion

    #region -- RefreshToken()
    /// <summary>
    /// 刷新Token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponseDto<object>.Error("Invalid token", 401));
        }

        var result = await _userService.RefreshTokenAsync(userId, request.RefreshToken);
        return StatusCode(result.StatusCode, result);
    }
    #endregion

    #region -- Recharge()
    /// <summary>
    /// 账户充值（需要认证）
    /// </summary>
    [Authorize]
    [HttpPost("recharge")]
    public async Task<IActionResult> Recharge([FromBody] RechargeRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userService.RechargeAsync(userId, request);
        return StatusCode(result.StatusCode, result);
    }

    #endregion

    #region -- ChangePassword()
    /// <summary>
    /// 修改密码（需要认证）
    /// </summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userService.ChangePasswordAsync(userId, request);
        return StatusCode(result.StatusCode, result);
    }

    #endregion

    #region -- Logout()
    /// <summary>
    /// 退出登录（需要认证）
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userService.LogoutAsync(userId);
        return StatusCode(result.StatusCode, result);
    }
    #endregion

    #region -- GetProfile()
    /// <summary>
    /// 获取用户信息（需要认证）
    /// </summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userService.GetUserInfoAsync(userId);
        return StatusCode(result.StatusCode, result);
    }
    #endregion

    #region -- GetBalance()
    /// <summary>
    /// 获取账户余额（需要认证）
    /// </summary>
    [Authorize]
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userService.GetBalanceAsync(userId);
        return StatusCode(result.StatusCode, result);
    }
    #endregion
}
