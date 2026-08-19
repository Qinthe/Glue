using Glue.API.Models.Dtos.Setting;

namespace Glue.API.Services.Interfaces;

public interface IUserSettingsService
{
    Task<UserSettingDto> GetUserSettingsAsync(string userId);
    Task<UserSettingDto> CreateDefaultSettingsAsync(string userId);
    Task<UserSettingDto> UpdateSettingsAsync(string userId, UserSettingRequestDto requestDto);
    Task<UserSettingDto> ResetSettingsAsync(string userId);
}
