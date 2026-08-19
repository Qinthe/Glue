using MySqlConnector;
using System.Data;
using System.Data.Common;

namespace Glue.API.Database;

public class GlueDBConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public GlueDBConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("GlueConnection")
            ?? throw new InvalidOperationException("Connection string 'GlueConnection' not found.");
    }

    public DbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
