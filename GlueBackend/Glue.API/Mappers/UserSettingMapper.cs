using Glue.API.Database.Entities.GlueUser;
using Glue.API.Models.Dtos.Setting;
using System.Text.Json;

namespace Glue.API.Mappers;

public static class UserSettingMapper
{
    #region -- ToDto()
    /// <summary>
    /// 将 GlueUserSetting 实体转换为 UserSettingDto
    /// </summary>
    public static UserSettingDto ToDto(this GlueUserSetting entity)
    {
        IEnumerable<string>? notificationMethods = null;
        if (entity.NotificationMethodsJson != null)
        {
            try
            {
                notificationMethods = JsonSerializer.Deserialize<IEnumerable<string>>(entity.NotificationMethodsJson);
            }
            catch
            {
                notificationMethods = Enumerable.Empty<string>();
            }
        }

        return new UserSettingDto
        {
            UserId = entity.UserId,
            Locale = entity.Locale,
            Theme = entity.Theme,
            PrimaryColor = entity.PrimaryColor,
            HotkeyCombo = entity.HotkeyCombo,
            SidebarCollapsed = entity.SidebarCollapsed,
            LaunchAtStartup = entity.LaunchAtStartup,
            StartUpBehavior = entity.StartupBehavior,
            CloseButtonBehavior = entity.CloseButtonBehavior,
            DefaultView = entity.DefaultView,
            NoticationMethods = notificationMethods,
            DoNotDisturbEnabled = entity.DoNotDisturbEnabled,
            DoNotDisturbStart = entity.DoNotDisturbStart,
            DoNotDisturbEnd = entity.DoNotDisturbEnd ,
            UpdateCheckMode = entity.UpdateCheckMode,
            AutoCheckUpdateOnStartup = entity.AutoCheckUpdateOnStartup,
            UpdateChannel = entity.UpdateChannel,
            Use24HourTime = entity.Use24HourTime,
            ShowWeekNumber = entity.ShowWeekNumber,
            ConfirmBeforeExit = entity.ConfirmBeforeExit,
            ReduceAnimation = entity.ReduceAnimation,
            CompactMode = entity.CompactMode,
            FeedbackContact = entity.FeedbackContact,
            FeedbackMessage = entity.FeedbackMessage,
            ResetKeepData = entity.ResetKeepData
        };
    }
    #endregion

    #region -- RequestToDto
    public static UserSettingDto RequestToDto(this UserSettingRequestDto request)
    {
        return new UserSettingDto
        {
            UserId = request.UserId,
            Locale = request.Locale,
            Theme = request.Theme,
            PrimaryColor = request.PrimaryColor,
            HotkeyCombo = request.HotkeyCombo,
            SidebarCollapsed = request.SidebarCollapsed,
            LaunchAtStartup = request.LaunchAtStartup,
            StartUpBehavior = request.StartUpBehavior,
            CloseButtonBehavior = request.CloseButtonBehavior,
            DefaultView = request.DefaultView,
            NoticationMethods = request.NoticationMethods,
            DoNotDisturbEnabled = request.DoNotDisturbEnabled,
            DoNotDisturbStart = request.DoNotDisturbStart,
            DoNotDisturbEnd = request.DoNotDisturbEnd,
            UpdateCheckMode = request.UpdateCheckMode,
            AutoCheckUpdateOnStartup = request.AutoCheckUpdateOnStartup,
            UpdateChannel = request.UpdateChannel,
            Use24HourTime = request.Use24HourTime,
            ShowWeekNumber = request.ShowWeekNumber,
            ConfirmBeforeExit = request.ConfirmBeforeExit,
            ReduceAnimation = request.ReduceAnimation,
            CompactMode = request.CompactMode,
            FeedbackContact = request.FeedbackContact,
            FeedbackMessage = request.FeedbackMessage,
            ResetKeepData = request.ResetKeepData
        };
    }
    #endregion

    #region -- ApplyUpdate()
    /// <summary>
    /// 将 UpdateUserSettingRequest 应用到现有实体（部分更新）
    /// </summary>
    public static void ApplyUpdate(this GlueUserSetting entity, UserSettingDto request)
    {
        if (request.Locale != null) entity.Locale = request.Locale;
        if (request.Theme != null) entity.Theme = request.Theme;
        if (request.HotkeyCombo != null) entity.HotkeyCombo = request.HotkeyCombo;
        if (request.PrimaryColor != null) entity.PrimaryColor = request.PrimaryColor;
        if (request.SidebarCollapsed) entity.SidebarCollapsed = request.SidebarCollapsed;
        if (request.LaunchAtStartup) entity.LaunchAtStartup = request.LaunchAtStartup;
        if (request.StartUpBehavior != null) entity.StartupBehavior = request.StartUpBehavior;
        if (request.CloseButtonBehavior != null) entity.CloseButtonBehavior = request.CloseButtonBehavior;
        if (request.DefaultView != null) entity.DefaultView = request.DefaultView;

        if (request.NoticationMethods != null)
        {
            var json = JsonSerializer.Serialize(request.NoticationMethods);
            entity.NotificationMethodsJson = JsonDocument.Parse(json);
        }

        if (request.DoNotDisturbEnabled) entity.DoNotDisturbEnabled = request.DoNotDisturbEnabled;
        if (request.DoNotDisturbStart != null) entity.DoNotDisturbStart = request.DoNotDisturbStart;
        if (request.DoNotDisturbEnd != null) entity.DoNotDisturbEnd = request.DoNotDisturbEnd;
        if (request.UpdateCheckMode != null) entity.UpdateCheckMode = request.UpdateCheckMode;
        if (request.AutoCheckUpdateOnStartup) entity.AutoCheckUpdateOnStartup = request.AutoCheckUpdateOnStartup;
        if (request.UpdateChannel != null) entity.UpdateChannel = request.UpdateChannel;
        if (request.TimeZone != null) entity.TimeZone = request.TimeZone;
        if (request.Use24HourTime) entity.Use24HourTime = request.Use24HourTime;
        if (request.ShowWeekNumber) entity.ShowWeekNumber = request.ShowWeekNumber;
        if (request.ConfirmBeforeExit) entity.ConfirmBeforeExit = request.ConfirmBeforeExit;
        if (request.ReduceAnimation) entity.ReduceAnimation = request.ReduceAnimation;
        if (request.CompactMode) entity.CompactMode = request.CompactMode;
        if (request.FeedbackContact != null) entity.FeedbackContact = request.FeedbackContact;
        if (request.FeedbackMessage != null) entity.FeedbackMessage = request.FeedbackMessage;
        if (request.ResetKeepData) entity.ResetKeepData = request.ResetKeepData;
    }
    #endregion

    #region -- CreateDefault()
    /// <summary>
    /// 创建默认用户设置实体
    /// </summary>
    public static GlueUserSetting CreateDefault(string userId)
    {
        return new GlueUserSetting
        {
            UserId = userId,
            Locale = "zh-CN",
            Theme = "auto",
            HotkeyCombo = "ctrl+space",
            PrimaryColor = "#409eff",
            SidebarCollapsed = true,
            LaunchAtStartup = false,
            StartupBehavior = "show-main",
            CloseButtonBehavior = "minimize-to-tray",
            DefaultView = "tasks",
            DoNotDisturbEnabled = false,
            DoNotDisturbStart = "22:00",
            DoNotDisturbEnd = "08:00",
            UpdateCheckMode = "auto",
            AutoCheckUpdateOnStartup = true,
            UpdateChannel = "stable",
            TimeZone = "Asia/Shanghai",
            Use24HourTime = true,
            ShowWeekNumber = false,
            ConfirmBeforeExit = true,
            ReduceAnimation = false,
            CompactMode = false,
            ResetKeepData = true
        };
    }
    #endregion
}
