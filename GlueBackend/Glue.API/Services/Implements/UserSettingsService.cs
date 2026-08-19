using Glue.API.Mappers;
using Glue.API.Models.Dtos.Setting;
using Glue.API.Repositories.Interfaces;
using Glue.API.Services.Interfaces;

namespace Glue.API.Services.Implements;

public class UserSettingsService : IUserSettingsService
{
    private readonly IUserSettingsRepository _repository;

    public UserSettingsService(IUserSettingsRepository repository)
    {
        _repository = repository;
    }

    #region -- GetUserSettingsAsync()
    public async Task<UserSettingDto> GetUserSettingsAsync(string userId)
    {
        var settings = await _repository.GetByUserIdAsync(userId);

        // 如果不存在，创建默认设置
        if (settings == null)
        {
            return await CreateDefaultSettingsAsync(userId);
        }

        return settings.ToDto();
    }
    #endregion

    #region -- CreateDefaultSettingsAsync()
    public async Task<UserSettingDto> CreateDefaultSettingsAsync(string userId)
    {
        var settings = UserSettingMapper.CreateDefault(userId);
        await _repository.CreateAsync(settings);
        return settings.ToDto();
    }
    #endregion

    #region -- UpdateSettingsAsync()
    public async Task<UserSettingDto> UpdateSettingsAsync(string userId, UserSettingRequestDto requestDto)
    {
        var dto = UserSettingMapper.RequestToDto(requestDto);
        var settings = await _repository.GetByUserIdAsync(userId);

        if (settings == null)
        {
            // 如果设置不存在，先创建默认设置
            settings = UserSettingMapper.CreateDefault(userId);
        }


        UserSettingMapper.ApplyUpdate(settings, dto);
        settings.UserId = userId;

        await _repository.UpdateAsync(settings);

        return settings.ToDto();
    }
    #endregion

    #region -- ResetSettingsAsync()
    public async Task<UserSettingDto> ResetSettingsAsync(string userId)
    {
        var existingSettings = await _repository.GetByUserIdAsync(userId);

        if (existingSettings != null)
        {
            await _repository.DeleteAsync(userId);
        }

        var defaultSettings = UserSettingMapper.CreateDefault(userId);
        await _repository.CreateAsync(defaultSettings);

        return defaultSettings.ToDto();
    }
    #endregion
}
