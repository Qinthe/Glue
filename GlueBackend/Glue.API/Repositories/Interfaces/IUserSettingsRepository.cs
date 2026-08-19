using Glue.API.Database.Entities.GlueUser;

namespace Glue.API.Repositories.Interfaces;

public interface IUserSettingsRepository : IBaseRepository<GlueUserSetting>
{
    Task<GlueUserSetting?> GetByUserIdAsync(string userId);
}
