USE glue_db;

-- ------------------------------------------------------------------
-- glue_tabs 快捷标签
-- ------------------------------------------------------------------
drop table if exists ue_tabs;
create table if not exists glue_tabs (
    -- 主键字段
    id                      varchar(128)                 not null,

    user_id                 varchar(128)                 not null,                              -- 用户id

    title                   varchar(128)                 not null,                              -- 主题
    url                     varchar(2048)                not null,                              -- url
    icon                    varchar(2048)                null,                                  -- 图标
    image                   varchar(2048)                null,                                  -- 图片
    category                varchar(128)                 not null,                              -- 类型
    open_mode               tinyint                      not null,                              -- open模式
    sort_order              int                          not null default 0,                    -- 排序
    is_pinned               tinyint(1)                   not null default 0,                    -- 固定
    description             varchar(512)                 null,                                  -- 描述
    color                   varchar(128)                 null,                                  -- 颜色

    -- 时间戳字段
    created_at              datetime                     not null default CURRENT_TIMESTAMP,
    updated_at              datetime                     not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    PRIMARY KEY (id),
    KEY ix_tabs_user_sort (user_id, sort_order)
);