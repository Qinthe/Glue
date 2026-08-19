namespace Glue.API.Models.Dtos.Setting;

public class UserSettingDto
{
    public required string UserId { get; set; }
    public required string Locale { get; set; }
    public required string Theme { get; set; }
    public required string HotkeyCombo { get; set; }
    public required string PrimaryColor { get; set; }
    public bool SidebarCollapsed { get; set; }

    public bool LaunchAtStartup { get; set; }
    public required string StartUpBehavior { get; set; }
    public string? CloseButtonBehavior { get; set; }

    public string? DefaultView { get; set; }
    public IEnumerable<string>? NoticationMethods { get; set; }
    public bool DoNotDisturbEnabled { get; set; }
    public string? DoNotDisturbStart { get; set; }
    public string? DoNotDisturbEnd { get; set; }
    public string? UpdateCheckMode { get; set; }
    public bool AutoCheckUpdateOnStartup { get; set; }
    public string? UpdateChannel { get; set; }
    public string? TimeZone { get; set; }
    public bool Use24HourTime { get; set; }
    public bool ShowWeekNumber { get; set; }
    public bool ConfirmBeforeExit { get; set; }
    public bool ReduceAnimation { get; set; }
    public bool CompactMode { get; set; }
    public string? FeedbackContact { get; set; }
    public string? FeedbackMessage { get; set; }
    public bool ResetKeepData { get; set; }
}
