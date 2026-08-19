using System.Data.Common;

namespace Glue.API.Database;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}
