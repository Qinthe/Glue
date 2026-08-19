using Glue.API.Database.Entities.GlueUser;

namespace Glue.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateAsync(GlueUser user);
    Task<GlueUser?> GetByIdAsync(string id);
    Task<GlueUser?> GetByEmailAsync(string email);
    Task<GlueUser?> GetByUserNameAsync(string userName);
    Task<int> UpdateBalanceAsync(string userId, decimal amount);
    Task<int> UpdateLastLoginAsync(string userId, string ipAddress);
    Task<int> UpdateLoginFailedAsync(string userId, bool isSuccess);
    Task<int> UpdatePasswordAsync(string userId, string newPasswordHash);
    Task<int> UpdateTokenVersionAsync(string userId);
    Task<int> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime expiry);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UserNameExistsAsync(string userName);
}