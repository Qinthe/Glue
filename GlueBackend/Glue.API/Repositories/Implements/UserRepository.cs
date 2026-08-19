namespace Glue.API.Repositories.Implements;
using Dapper;
using Glue.API.Database;
using Glue.API.Database.Entities.GlueUser;
using Glue.API.Repositories.Interfaces;

public class UserRepository : BaseRepository<GlueUser>, IUserRepository
{
    protected override string TableName => "glue_users";
    protected override string PrimaryKey => "id";

    public UserRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    #region -- GetByIdAsync()
    public async Task<GlueUser?> GetByIdAsync(string id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM glue_users WHERE id = @Id";

        return await connection.QueryFirstOrDefaultAsync<GlueUser>(sql, new { Id = id });
    }
    #endregion

    #region -- GetByEmailAsync()
    public async Task<GlueUser?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
            SELECT * 
            FROM glue_users 
            WHERE email = @Email";

        return await connection.QueryFirstOrDefaultAsync<GlueUser>(sql, new { Email = email });
    }

    #endregion

    #region -- GetByUserNameAsync()
    public async Task<GlueUser?> GetByUserNameAsync(string userName)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT * FROM glue_users WHERE user_name = @UserName";
        
        return await connection.QueryFirstOrDefaultAsync<GlueUser>(sql, new { UserName = userName });
    }
    #endregion

    #region -- CreateAsync()
    public override async Task<bool> CreateAsync(GlueUser user)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                INSERT INTO glue_users (id, user_name, email, password_hash, balance, is_active, role, created_at, updated_at)
                VALUES (@Id, @UserName, @Email, @PasswordHash, @Balance, @IsActive, @Role, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(sql, user);

        return true;
    }
    #endregion

    #region -- UpdateAsync()
    public override async Task<bool> UpdateAsync(GlueUser user)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                UPDATE glue_users 
                SET user_name = @UserName, 
                    email = @Email, 
                    is_active = @IsActive, 
                    role = @Role
                WHERE id = @Id";

        
        await connection.ExecuteAsync(sql, user);

        return true;
    }
    #endregion

    #region -- UpdateBalanceAsync()
    public async Task<int> UpdateBalanceAsync(string userId, decimal amount)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                UPDATE glue_users 
                SET balance = balance + @Amount
                WHERE id = @UserId AND is_active = 1";
        
        return await connection.ExecuteAsync(sql, new { UserId = userId, Amount = amount });
    }
    #endregion

    #region -- UpdateLastLoginAsync()
    public async Task<int> UpdateLastLoginAsync(string userId, string ipAddress)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                UPDATE glue_users 
                SET last_login_at = NOW(),
                    last_login_ip = @IpAddress,
                    login_failed_count = 0,
                    lockout_until = NULL
                WHERE id = @UserId";

        return await connection.ExecuteAsync(sql, new { UserId = userId, IpAddress = ipAddress });
    }
    #endregion

    #region -- UpdateLoginFailedAsync()
    public async Task<int> UpdateLoginFailedAsync(string userId, bool isSuccess)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql;
        if (isSuccess)
        {
            sql = @"
                    UPDATE glue_users 
                    SET login_failed_count = 0,
                        lockout_until = NULL
                    WHERE id = @UserId";
        }
        else
        {
            sql = @"
                    UPDATE glue_users 
                    SET login_failed_count = login_failed_count + 1,
                        lockout_until = CASE 
                            WHEN login_failed_count + 1 >= 5 THEN DATE_ADD(NOW(), INTERVAL 30 MINUTE)
                            ELSE NULL
                        END
                    WHERE id = @UserId";
        }

        return await connection.ExecuteAsync(sql, new { UserId = userId });
    }
    #endregion

    #region -- UpdatePasswordAsync()
    public async Task<int> UpdatePasswordAsync(string userId, string newPasswordHash)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                UPDATE glue_users 
                SET password_hash = @NewPasswordHash,
                    token_version = token_version + 1
                WHERE id = @UserId";

        return await connection.ExecuteAsync(sql, new { UserId = userId, NewPasswordHash = newPasswordHash });
    }
    #endregion

    #region -- UpdateTokenVersionAsync()
    public async Task<int> UpdateTokenVersionAsync(string userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                UPDATE glue_users 
                SET token_version = token_version + 1
                WHERE id = @UserId";

        return await connection.ExecuteAsync(sql, new { UserId = userId });
    }
    #endregion

    #region -- UpdateRefreshTokenAsync()
    public async Task<int> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime expiry)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
                UPDATE glue_users 
                SET refresh_token = @RefreshToken,
                    refresh_token_expiry = @Expiry
                WHERE id = @UserId";

        return await connection.ExecuteAsync(sql, new { UserId = userId, RefreshToken = refreshToken, Expiry = expiry });
    }
    #endregion

    #region -- EmailExistsAsync()
    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT COUNT(1) FROM glue_users WHERE email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });

        return count > 0;
    }
    #endregion

    #region -- UserNameExistsAsync()
    public async Task<bool> UserNameExistsAsync(string userName)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT COUNT(1) FROM glue_users WHERE user_name = @UserName";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserName = userName });
        return count > 0;
    }
    #endregion
}
