use glue_db;

-- ------------------------------------------------------------------
-- glue_tasks 任务
-- ------------------------------------------------------------------
drop table if exists glue_tasks;
create table if not exists glue_tasks (
    -- 主键字段
    id                          varchar(128)                    not null,

    user_id                     varchar(128)                    not null,                               -- 用户id

    title                       varchar(256)                    not null,                               -- 用户id
    description                 text                            null ,                                  -- 描述
    scheduled_date              date                            not null,                               -- 日期
    start_at                    datetime                        not null,                               -- 开始日期
    end_at                      datetime                        not null,                               -- 结束日期
    progress                    tinyint unsigned                not null,                               -- 进度
    status                      varchar(128)                    not null,                               -- 状态
    reminder_enabled            tinyint(1)                      not null,                               -- 是否提醒
    reminder_minutes_before     int                             not null,                               -- 距离提醒剩余时间
    last_reminder_at            datetime                        null,                                   -- 最近一次提醒时间
    completed_at                datetime                        null,                                   -- 是否完成

    -- 时间戳字段
    created_at                  datetime                        not null default CURRENT_TIMESTAMP,
    updated_at                  datetime                        not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    primary key (id),
    key ix_tasks_user_status (user_id, status),
    key ix_tasks_user_start_end (user_id, start_at, end_at)
);



-- ------------------------------------------------------------------
-- glue_tasks_groups 任务分组
-- ------------------------------------------------------------------
drop table if exists glue_tasks_groups;
create table if not exists glue_tasks_groups (
    -- 主键字段
    id                          varchar(128)                    not null,

    user_id                     varchar(128)                    not null,                              -- 用户id

    name                        varchar(256)                    not null,                              -- 用户id
    color                       nvarchar(32)                    not null,
    description                 text                            null,                                  -- 描述
    sort_order                  int                             not null,

    -- 时间戳字段
    created_at                  datetime                        not null default CURRENT_TIMESTAMP,
    updated_at                  datetime                        not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    primary key (id),
    key ix_taskgroups_user_name (user_id, name)
);



-- ------------------------------------------------------------------
-- glue_tasks_group_links 任务分组关联
-- ------------------------------------------------------------------
drop table if exists glue_tasks_group_links;
create table if not exists glue_tasks_group_links (
    -- 主键字段
    task_id                     varchar(128)                    not null,
    group_id                    varchar(128)                    not null,

    name                        varchar(256)                    not null,                              -- 用户id
    color                       nvarchar(32)                    not null,
    description                 text                            null,                                  -- 描述
    sort_order                  int                             not null,

    -- 时间戳字段
    created_at                  datetime                        not null default CURRENT_TIMESTAMP,
    updated_at                  datetime                        not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    primary key (task_id,group_id)
);

