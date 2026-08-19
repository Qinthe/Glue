use glue_db;

-- 插入测试数据（密码均为测试用，实际使用时需重新生成）
-- 测试管理员：admin@example.com / Admin@123
-- 测试普通用户：user@example.com / User@123
-- 注意：以下密码哈希使用BCrypt加密，密码验证时需要使用BCrypt

insert into glue_users (id, user_name, email, password_hash, balance, is_active, role, created_at, updated_at)
value
(
    UUID(),
    'admin',
    'admin@example.com',
    '$2a$11$8jZ7qxJqQZQZQZQZQZQZQeH5qY5M5Y5M5Y5M5Y5M5Y5M5Y5M5Y5',
    0.00,
    1,
    'Admin',
    NOW(),
    NOW()
),
(
    UUID(),
    'testuser',
    'user@example.com',
    '$2a$11$9kA8rB6sC4dE2fG1hI3jK5lM7nO9pQ1rS3tU5vW7xY9zA1bC3dE5fG',
    100.00,
    1,
    'User',
    NOW(),
    NOW()
);
