use glue_db;

-- ------------------------------------------------------------------
-- glue_users 用户表
-- ------------------------------------------------------------------
drop table if exists glue_users;
create table if not exists glue_users (
    -- 主键字段
    id                      varchar(128)                not null,

    -- 基本信息字段
    user_name               varchar(128)                not null,
    email                   varchar(256)                not null,
    password_hash           varchar(255)                not null,
    avatar_url              varchar(512)                null,                                   -- 头像URL
    nick_name               varchar(128)                null,                                  -- 显示昵称（区别于登录名）
    real_name               varchar(128)                null,                                   -- 真实姓名
    signature               varchar(256)                null,                                   -- 个性签名/个人简介
    phone_number            varchar(32)                 null,                                   -- 手机号

    -- 安全与验证类
    email_verified          tinyint(1)                  not null default 0,                     -- 邮箱是否已验证
    phone_verified          tinyint(1)                  not null default 0,                     -- 手机是否已验证
    two_factor_secret       varchar(255)                null,                                   -- 双因素认证密钥

    -- JWT安全增强字段（可选）
    token_version           int                         not null default 1,                     -- Token版本号（修改密码或注销时递增，使旧token失效）
    refresh_token           varchar(512)                null,                                   -- 刷新令牌（用于获取新的access token）
    refresh_token_expiry    datetime                    null,                                   -- 刷新令牌过期时间

    -- 账户相关字段
    balance                 decimal(10,2)               not null default 0.00,                  -- 账户余额（单位：元，最大99,999,999.99）
    is_active               tinyint(1)                  not null default 1,                     -- 账户状态（1：激活，0：禁用）
    role                    varchar(128)                not null default 'User',                -- 用户角色（User：普通用户，Admin：管理员）

    -- 登录相关字段
    last_login_at           datetime                    null,                                   -- 最后登录时间'
    last_login_ip           varchar(128)                null,                                   -- 最后登录IP地址（支持IPv6）
    login_failed_count      int                         not null default 0,                     -- 连续登录失败次数
    lockout_until           datetime                    null,                                   -- 账户锁定截止时间

    -- 时间戳字段
    created_at              datetime                    not null default CURRENT_TIMESTAMP ,
    updated_at              datetime                    not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    -- 主键约束
    primary key (id),

    -- 唯一约束
    unique key uk_users_email (email),                                                          -- 邮箱唯一索引
    unique key uk_users_username (user_name),                                                   -- 用户名唯一索引

    -- 普通索引
    index idx_users_email (email),                                                              -- 邮箱查询索引
    index idx_users_is_active (is_active),                                                      -- 账户状态索引
    index idx_users_role (role),                                                                -- 用户角色索引
    index idx_users_created_at (created_at),                                                    -- 创建时间索引
    index idx_users_token_version (token_version),                                              -- Token版本索引
    index idx_users_lockout (lockout_until)                                                     -- 账户锁定查询索引

);


-- ------------------------------------------------------------------
-- glue_token_blacklist 创建Token黑名单表（用于退出登录和强制下线）
-- ------------------------------------------------------------------
drop table IF exists glue_token_blacklist;
create table if not exists glue_token_blacklist (
    -- 主键
    id                      varchar(128)                not null,

    token_jti               varchar(256)                not null,                                -- Token的唯一标识（JWT ID）
    user_id                 varchar(128)                not null,
    expires_at              datetime                    not null,
    -- Token过期时间',
    created_at              datetime                    not null default CURRENT_TIMESTAMP,      -- 加入黑名单时间,

    primary key (id),
    unique key uk_token_jti (token_jti),                                                         -- Token唯一索引
    index idx_tokenblacklist_user (user_id),                                                     -- 用户ID索引
    index idx_tokenblacklist_expires (expires_at)                                                -- 过期时间索引，用于清理
);



-- ------------------------------------------------------------------
-- glue_user_settings 用户设置
-- ------------------------------------------------------------------
drop table if exists glue_user_settings;
create table if not exists glue_user_settings (
    -- 主键
    user_id                         varchar(128)                    not null,                              -- 用户id

    locale                          varchar(128)                    not null default 'zh-CN' ,             -- 语言/地区设置（如 zh-CN、en-US）
    theme                           varchar(128)                    not null default 'auto',               -- 主题（浅色/深色/跟随系统）
    hotkey_combo                    varchar(128)                    not null default 'ctrl+space',         -- 全局快捷键组合键（如 Ctrl+Shift+G）
    primary_color                   varchar(128)                    not null default '#409eff',            -- 主题主色调
    sidebar_collapsed               tinyint(1)                      not null default 1,                    -- 侧边栏是否折叠（1=折叠，0=展开）

    launch_at_startup               tinyint(1)                      not null default 0,                    -- 是否开机自启
    startup_behavior                varchar(128)                    not null default 'show-main',          -- 启动时行为（如 show-main、minimize 等）
    close_button_behavior           varchar(128)                    not null default 'minimize-to-tray',   -- 点击关闭按钮行为（如 minimize-to-tray、exit 等）

    default_view                    varchar(128)                    not null,                              -- 默认打开的页面（如 tasks、calendar 等）

    notification_methods_json       json                            null,                                  -- 通知方式配置（JSON格式，如弹窗、声音等）
    do_not_disturb_enabled          tinyint(1)                      not null default 0,                    -- 是否启用勿扰模式
    do_not_disturb_start            varchar(128)                    not null default '22:00',              -- 勿扰模式开始时间（如 22:00）
    do_not_disturb_end              varchar(128)                    not null default '08:00',              -- 勿扰模式结束时间（如 08:00）

    update_check_mode               varchar(128)                    not null  default 'auto',              -- 更新检查模式（auto/manual）
    auto_check_update_on_startup    tinyint(1)                      not null default 1,                    -- 启动时是否自动检查更新
    update_channel                  varchar(128)                    not null default 'stable',             -- 更新通道（stable/beta/dev）

    timezone                        varchar(64)                     not null default 'Asia/Shanghai',      -- 时区
    use_24_hour_time                tinyint(1)                      not null default 1,                    -- 是否使用24小时制
    show_week_number                tinyint(1)                      not null default 0,                    -- 是否显示周数
    confirm_before_exit             tinyint(1)                      not null default 1,                    -- 退出前是否需要确认
    reduce_animation                tinyint(1)                      not null default 0,                    -- 是否减少动画效果（辅助功能）
    compact_mode                    tinyint(1)                      not null default 0,                    -- 是否启用紧凑模式

    feedback_contact                varchar(128)                    null,                                  -- 反馈联系方式（邮箱等）
    feedback_message                text                            null,                                  -- 反馈消息内容
    reset_keep_data                 tinyint(1)                      not null default 1,                    -- 重置设置时是否保留用户数据

    -- 时间戳字段
    created_at                      datetime                        not null default CURRENT_TIMESTAMP,
    updated_at                      datetime                        not null default CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (user_id)
);
















