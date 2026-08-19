use glue_db;

-- ------------------------------------------------------------------
-- glue_notifications 通知
-- ------------------------------------------------------------------
drop table if exists glue_notifications;
create table if not exists glue_notifications (
    -- 主键字段
    id                      varchar(128)                 not null,

    user_id                 varchar(128)                 not null,                              -- 用户id

    kind                    varchar(128)                 not null,                              -- 分类
    level                   varchar(20)                  not null,                              -- 级别
    title                   varchar(256)                 not null,                              -- 主题
    message                 varchar(2048)                not null,                              -- 消息
    releate_id              varchar(128)                 null,                                  -- 关联id
    read_at                 datetime                     null,

    -- 时间戳字段
    created_at              datetime                     not null default CURRENT_TIMESTAMP,
    updated_at              datetime                     not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,


    primary key (id),
    key ix_notifications_user_read_created (user_id, read_at, created_at),
    key ix_notifications_user_task (user_id, releate_id)
);