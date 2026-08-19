using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Glue.API.Controllers;

[ApiController]
[Authorize] // 添加全局授权，所有继承的控制器都需要认证
public abstract class BaseController : ControllerBase
{

    #region -- GetUserId()
    /// <summary>
    /// 获取当前登录用户的 ID
    /// </summary>
    protected string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("nameid")?.Value
            ?? User.FindFirst("userId")?.Value
            ?? throw new UnauthorizedAccessException("用户未登录");
    }
    #endregion

    #region -- GetUserEmail()
    /// <summary>
    /// 获取当前登录用户的邮箱
    /// </summary>
    protected string? GetUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value;
    }
    #endregion

    #region -- GetUserRole()
    /// <summary>
    /// 获取当前登录用户的角色
    /// </summary>
    protected string? GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }
    #endregion
}