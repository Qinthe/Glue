using System.Text.Json;

namespace Glue.API.Database.Entities.GlueUser;

public class GlueUserSetting : BaseEntity
{
    public required string UserId { get; set; }

    public required string Locale { get; set; } = "zh-CN";
    public required string Theme { get; set; } = "auto";
    public required string HotkeyCombo { get; set; } = "ctrl+space";
    public required string PrimaryColor { get; set; } = "#409eff";
    public required bool SidebarCollapsed { get; set; } = true;

    public required bool LaunchAtStartup { get; set; } = false;
    public required string StartupBehavior { get; set; } = "show-main";
    public required string CloseButtonBehavior { get; set; } = "minimize-to-tray";

    public required string DefaultView { get; set; } = "tasks";

    public JsonDocument? NotificationMethodsJson { get; set; }
    public bool DoNotDisturbEnabled { get; set; } = false;
    public string DoNotDisturbStart { get; set; } = "22:00";
    public string DoNotDisturbEnd { get; set; } = "08:00";

    public string UpdateCheckMode { get; set; } = "auto";
    public bool AutoCheckUpdateOnStartup { get; set; } = true;
    public string UpdateChannel { get; set; } = "stable";

    public required string TimeZone { get; set; } = "Asia/Shanghai";
    public bool Use24HourTime { get; set; } = true;
    public bool ShowWeekNumber { get; set; } = false;
    public bool ConfirmBeforeExit { get; set; } = true;
    public bool ReduceAnimation { get; set; } = false;
    public bool CompactMode { get; set; } = false;

    public string? FeedbackContact { get; set; }
    public string? FeedbackMessage { get; set; }
    public bool ResetKeepData { get; set; } = true;
}
