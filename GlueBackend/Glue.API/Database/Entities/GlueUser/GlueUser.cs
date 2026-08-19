using System.ComponentModel.DataAnnotations.Schema;

namespace Glue.API.Database.Entities.GlueUser;

public class GlueUser : BaseEntity
{
    //基本信息字段
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? AvatarUrl { get; set; }
    public string? NickName { get; set; }
    public string? RealName { get; set; }
    public string? Signature { get; set; }
    public string? PhoneNumber { get; set; }

    //安全与验证类
    public bool EmailVerified { get; set; } = false;
    public bool PhotelVerified { get; set; }= false;
    public string? TwoFactorSecret { get; set; }

    //JWT安全增强字段
    public int TokenVersion { get; set; } = 1;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    //账户相关字段
    public decimal Balance { get; set; }=0.00m;
    public required bool IsActive { get; set; } =true;
    public required string Role { get; set; }

    //登录相关字段
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int LoginFailedCount { get; set; }
    public DateTime? LockoutUntil { get; set; }
}
