using Glue.API.Database.Entities.GlueUser;
using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.User;

namespace Glue.API.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponseDto<object>> RegisterAsync(RegisterRequestDto request, string ipAddress);
    Task<ApiResponseDto<object>> LoginAsync(LoginRequestDto request, string ipAddress);
    Task<ApiResponseDto<object>> RefreshTokenAsync(string userId, string refreshToken);
    Task<ApiResponseDto<object>> RechargeAsync(string userId, RechargeRequestDto request);
    Task<ApiResponseDto<object>> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
    Task<ApiResponseDto<object>> LogoutAsync(string userId);
    Task<ApiResponseDto<GlueUser>> GetUserInfoAsync(string userId);
    Task<ApiResponseDto<object>> GetBalanceAsync(string userId);
}