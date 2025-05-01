using Microsoft.Data.SqlClient;
using System.Data;

namespace AuthWithStorage.Infrastructure.Data
{
    public class DbContext(string connectionString)
    {
        public IDbConnection CreateConnection()
            => new SqlConnection(connectionString);
    }
}
