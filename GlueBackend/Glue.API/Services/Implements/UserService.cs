using Glue.API.Database.Entities.GlueUser;
using Glue.API.Models.Dtos;
using Glue.API.Models.Dtos.User;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services.Interfaces;
using Glue.API.Utils;

namespace Glue.API.Services.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;
    public UserService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    #region -- RegisterAsync()
    public async Task<ApiResponseDto<object>> RegisterAsync(RegisterRequestDto request, string ipAddress)
    {
        try
        {
            // 验证邮箱是否已存在
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return ApiResponseDto<object>.Error("Email already exists", 400, "EMAIL_ALREADY_EXISTS");
            }

            // 验证用户名是否已存在
            if (await _userRepository.UserNameExistsAsync(request.UserName))
            {
                return ApiResponseDto<object>.Error("Username already exists", 400, "USERNAME_ALREADY_EXISTS");
            }

            var user = new GlueUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = PasswordUtil.HashPassword(request.Password),
                Balance = 0,
                IsActive = true,
                Role = "User",
                TokenVersion = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userRepository.CreateAsync(user);

            if (result)
            {
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

                return ApiResponseDto<object>.Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new { user.Id, user.UserName, user.Email, user.Balance, user.Role }
                }, "Registration successful", "REGISTRATION_SUCCESS");
            }

            return ApiResponseDto<object>.Error("Registration failed", 500,"REGISTRATION_FAILED");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Registration failed: {ex.Message}", 500);
        }
    }
    #endregion

    #region -- LoginAsync()
    public async Task<ApiResponseDto<object>> LoginAsync(LoginRequestDto request, string ipAddress)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponseDto<object>.Error("Invalid email or password", 401, "USER_NOT_EXIST");
            }

            // 检查账户是否锁定
            if (user.LockoutUntil.HasValue && user.LockoutUntil > DateTime.UtcNow)
            {
                var remainingMinutes = (user.LockoutUntil.Value - DateTime.UtcNow).Minutes;
                return ApiResponseDto<object>.Error($"Account is locked. Please try again in {remainingMinutes} minutes", 403, "ACCOUNT_LOCKED", new { RemainingMinutes = remainingMinutes });
            }

            if (!user.IsActive)
            {
                return ApiResponseDto<object>.Error("Account is deactivated", 403, "ACCOUNT_DEACTIVATED");
            }

            if (!PasswordUtil.VerifyPassword(request.Password, user.PasswordHash))
            {
                await _userRepository.UpdateLoginFailedAsync(user.Id, false);
                var remainingAttempts = 5 - (user.LoginFailedCount + 1);
                return ApiResponseDto<object>.Error($"Invalid email or password. {remainingAttempts} attempts remaining", 401, "INCORRECT_PASSWORD");
            }

            // 更新最后登录信息
            await _userRepository.UpdateLastLoginAsync(user.Id, ipAddress);

            // 生成新令牌
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

            var userInfo = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Balance,
                user.Role,
                user.IsActive
            };

            return ApiResponseDto<object>.Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = userInfo
            }, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Login failed: {ex.Message}", 500, "LOGIN_FAILED");
        }
    }
    #endregion

    #region -- RefreshTokenAsync()
    public async Task<ApiResponseDto<object>> RefreshTokenAsync(string userId, string refreshToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponseDto<object>.Error("User not found", 404);
            }

            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return ApiResponseDto<object>.Error("Invalid or expired refresh token", 401);
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken, newRefreshTokenExpiry);

            return ApiResponseDto<object>.Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Token refresh failed: {ex.Message}", 500);
        }
    }
    #endregion

    #region -- RechargeAsync()
    public async Task<ApiResponseDto<object>> RechargeAsync(string userId, RechargeRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponseDto<object>.Error("User not found", 404);
            }

            if (!user.IsActive)
            {
                return ApiResponseDto<object>.Error("Account is deactivated", 403);
            }

            var result = await _userRepository.UpdateBalanceAsync(userId, request.Amount);

            if (result > 0)
            {
                var updatedUser = await _userRepository.GetByIdAsync(userId);
                return ApiResponseDto<object>.Ok(new { NewBalance = updatedUser.Balance }, $"Recharged {request.Amount:C} successfully");
            }

            return ApiResponseDto<object>.Error("Recharge failed", 500);
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Recharge failed: {ex.Message}", 500);
        }
    }
    #endregion

    #region -- ChangePasswordAsync()
    public async Task<ApiResponseDto<object>> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponseDto<object>.Error("User not found", 404);
            }

            if (!PasswordUtil.VerifyPassword(request.OldPassword, user.PasswordHash))
            {
                return ApiResponseDto<object>.Error("Old password is incorrect", 400);
            }

            var newPasswordHash = PasswordUtil.HashPassword(request.NewPassword);
            await _userRepository.UpdatePasswordAsync(userId, newPasswordHash);

            return ApiResponseDto<object>.Ok(null, "Password changed successfully. Please login again.");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Password change failed: {ex.Message}", 500);
        }
    }
    #endregion

    #region -- LogoutAsync()
    public async Task<ApiResponseDto<object>> LogoutAsync(string userId)
    {
        try
        {
            await _userRepository.UpdateRefreshTokenAsync(userId, null, DateTime.UtcNow);
            await _userRepository.UpdateTokenVersionAsync(userId);

            return ApiResponseDto<object>.Ok(null, "Logged out successfully", "LOGOUT_SUCCESS");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Logout failed: {ex.Message}", 500, "LOGOUT_FAILED");
        }
    }
    #endregion

    #region -- GetUserInfoAsync()
    public async Task<ApiResponseDto<GlueUser>> GetUserInfoAsync(string userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponseDto<GlueUser>.Error("User not found", 404);
            }

            // 隐藏敏感信息
            user.PasswordHash = null;
            user.RefreshToken = null;

            return ApiResponseDto<GlueUser>.Ok(user, "User info retrieved");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<GlueUser>.Error($"Failed to get user info: {ex.Message}", 500);
        }
    }

    #endregion

    #region -- GetBalanceAsync()
    public async Task<ApiResponseDto<object>> GetBalanceAsync(string userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponseDto<object>.Error("User not found", 404);
            }

            return ApiResponseDto<object>.Ok(new { Balance = user.Balance }, "Balance retrieved");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<object>.Error($"Failed to get balance: {ex.Message}", 500);
        }
    }
    #endregion
}
