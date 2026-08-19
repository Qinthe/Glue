using Dapper;
using Glue.API.Database;
using Glue.API.Database.Entities.GlueUser;
using Glue.API.Repositories.Interfaces;

namespace Glue.API.Repositories.Implements;

public class UserSettingsRepository : BaseRepository<GlueUserSetting>, IUserSettingsRepository
{
    protected override string TableName => "glue_user_settings";
    protected override string PrimaryKey => "user_id";

    public UserSettingsRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    #region -- GetByUserIdAsync()
    public async Task<GlueUserSetting?> GetByUserIdAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = "SELECT * FROM glue_user_settings WHERE user_id = @UserId";

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { UserId = userId });


        return result;

        /*
        if (result == null)
            return null;
        
        var settings = new UserSettings
        {
            UserId = result.user_id,
            Locale = result.locale,
            Theme = result.theme,
            HotkeyCombo = result.hotkey_combo,
            PrimaryColor = result.primary_color,
            SidebarCollapsed = Convert.ToBoolean(result.sidebar_collapsed),
            LaunchAtStartup = Convert.ToBoolean(result.launch_at_startup),
            StartupBehavior = result.startup_behavior,
            CloseButtonBehavior = result.close_button_behavior,
            DefaultView = result.default_view,
            DoNotDisturbEnabled = Convert.ToBoolean(result.do_not_disturb_enabled),
            DoNotDisturbStart = result.do_not_disturb_start,
            DoNotDisturbEnd = result.do_not_disturb_end,
            UpdateCheckMode = result.update_check_mode,
            AutoCheckUpdateOnStartup = Convert.ToBoolean(result.auto_check_update_on_startup),
            UpdateChannel = result.update_channel,
            Use24HourTime = Convert.ToBoolean(result.use_24_hour_time),
            ShowWeekNumber = Convert.ToBoolean(result.show_week_number),
            ConfirmBeforeExit = Convert.ToBoolean(result.confirm_before_exit),
            ReduceAnimation = Convert.ToBoolean(result.reduce_animation),
            CompactMode = Convert.ToBoolean(result.compact_mode),
            FeedbackContact = result.feedback_contact,
            FeedbackMessage = result.feedback_message,
            ResetKeepData = Convert.ToBoolean(result.reset_keep_data),
            CreatedAt = result.created_at,
            UpdatedAt = result.updated_at
        };

        // 处理JSON字段
        if (result.notification_methods_json != null)
        {
            string jsonStr = result.notification_methods_json.ToString();
            if (!string.IsNullOrEmpty(jsonStr))
            {
                settings.NotificationMethodsJson = JsonDocument.Parse(jsonStr);
            }
        }
         */
    }
    #endregion

    #region - CreateAsync()
    public override async Task<bool> CreateAsync(GlueUserSetting entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO glue_user_settings (
                user_id, locale, theme, hotkey_combo, primary_color, sidebar_collapsed,
                launch_at_startup, startup_behavior, close_button_behavior, default_view,
                notification_methods_json, do_not_disturb_enabled, do_not_disturb_start, do_not_disturb_end,
                update_check_mode, auto_check_update_on_startup, update_channel,
                use_24_hour_time, show_week_number, confirm_before_exit, reduce_animation, compact_mode,
                feedback_contact, feedback_message, reset_keep_data, created_at, updated_at
            ) VALUES (
                @UserId, @Locale, @Theme, @HotkeyCombo, @PrimaryColor, @SidebarCollapsed,
                @LaunchAtStartup, @StartupBehavior, @CloseButtonBehavior, @DefaultView,
                @NotificationMethodsJson, @DoNotDisturbEnabled, @DoNotDisturbStart, @DoNotDisturbEnd,
                @UpdateCheckMode, @AutoCheckUpdateOnStartup, @UpdateChannel,
                @Use24HourTime, @ShowWeekNumber, @ConfirmBeforeExit, @ReduceAnimation, @CompactMode,
                @FeedbackContact, @FeedbackMessage, @ResetKeepData, @CreatedAt, @UpdatedAt
            )";

        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await connection.ExecuteAsync(sql, entity);
        return true;
    }
    #endregion

    #region - UpdateAsync()
    public override async Task<bool> UpdateAsync(GlueUserSetting entity)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE glue_user_settings SET
                locale = @Locale,
                theme = @Theme,
                hotkey_combo = @HotkeyCombo,
                primary_color = @PrimaryColor,
                sidebar_collapsed = @SidebarCollapsed,
                launch_at_startup = @LaunchAtStartup,
                startup_behavior = @StartupBehavior,
                close_button_behavior = @CloseButtonBehavior,
                default_view = @DefaultView,
                notification_methods_json = @NotificationMethodsJson,
                do_not_disturb_enabled = @DoNotDisturbEnabled,
                do_not_disturb_start = @DoNotDisturbStart,
                do_not_disturb_end = @DoNotDisturbEnd,
                update_check_mode = @UpdateCheckMode,
                auto_check_update_on_startup = @AutoCheckUpdateOnStartup,
                update_channel = @UpdateChannel,
                use_24_hour_time = @Use24HourTime,
                show_week_number = @ShowWeekNumber,
                confirm_before_exit = @ConfirmBeforeExit,
                reduce_animation = @ReduceAnimation,
                compact_mode = @CompactMode,
                feedback_contact = @FeedbackContact,
                feedback_message = @FeedbackMessage,
                reset_keep_data = @ResetKeepData,
                updated_at = @UpdatedAt
            WHERE user_id = @UserId";

        entity.UpdatedAt = DateTime.UtcNow;

        var result = await connection.ExecuteAsync(sql, entity);
        return result > 0;
    }
    #endregion
}