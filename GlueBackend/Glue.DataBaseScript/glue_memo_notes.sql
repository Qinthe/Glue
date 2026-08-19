use glue_db;

-- ------------------------------------------------------------------
-- glue_memo_notes 备忘录
-- ------------------------------------------------------------------
drop table if exists glue_memo_notes;
create table if not exists glue_memo_notes (
    -- 主键字段
    id                      varchar(128)                 not null,

    user_id                 varchar(128)                 not null,                              -- 用户id

    title                   varchar(256)                 not null,                              -- 标题
    content                 text                         not null,                              -- 内容
    category                varchar(128)                 not null,                              -- 类型

    -- 时间戳字段
    created_at              datetime                     not null default CURRENT_TIMESTAMP,
    updated_at              datetime                     not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    primary key (id),
    key ix_memonotes_user_updated (user_id, updated_at)
);



-- ------------------------------------------------------------------
-- glue_memo_tags 备忘录标签
-- ------------------------------------------------------------------
drop table if exists glue_memo_tags;
create table if not exists glue_memo_tags (
    -- 主键字段
    memo_id                 varchar(128)                 not null,

    tag                     nvarchar(256)                not null,                              -- 标签

    -- 时间戳字段
    created_at              datetime                     not null default CURRENT_TIMESTAMP,
    updated_at              datetime                     not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP,

    primary key (memo_id,tag)
);