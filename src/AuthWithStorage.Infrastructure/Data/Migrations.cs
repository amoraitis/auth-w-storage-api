using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AuthWithStorage.Infrastructure.Data
{
    public static class Database
    {
        public static void EnsureCreated(string sysConnectionString, string databaseName)
        {
            using var connection = new SqlConnection(sysConnectionString);
            connection.Open();

            var query = $@"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
                    BEGIN
                        CREATE DATABASE [{databaseName}];
                    END";

            connection.Execute(query);
        }

        public static void EnsureMigrations(string connectionString, IConfiguration configuration)
        {
            
            var scriptKeys = new[]
            {
                SqlScriptsConfig.Users,
                SqlScriptsConfig.Roles,
                SqlScriptsConfig.UserRoles,
                SqlScriptsConfig.Files,
                SqlScriptsConfig.Permissions,
                SqlScriptsConfig.RolePermissions,
                SqlScriptsConfig.AuditLogs,
                SqlScriptsConfig.SCRIPT_Seed
            };

            var combinedQuery = string.Join(Environment.NewLine, scriptKeys
                .Select(scriptKey => configuration[scriptKey])
                .Where(query => !string.IsNullOrEmpty(query)));

            if (!string.IsNullOrEmpty(combinedQuery))
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                connection.Execute(combinedQuery);
            }

            var sps = new[]
            {
                SqlScriptsConfig.SP_GetUsersFiltered
            };

            foreach (var sp in sps)
            {
                var query = configuration[sp];
                if (!string.IsNullOrEmpty(query))
                {
                    var batches = query.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                    using var connection = new SqlConnection(connectionString);
                    connection.Open();

                    foreach (var batch in batches)
                    {
                        if (!string.IsNullOrWhiteSpace(batch))
                        {
                            using var command = new SqlCommand(batch.Trim(), connection);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

        }

        public static class SqlScriptsConfig
        {
            public static string Users => "dbo.Users.Table.sql";
            public static string Roles => "dbo.Roles.Table.sql";
            public static string UserRoles => "dbo.UserRoles.Table.sql";
            public static string UserTokens => "dbo.UserTokens.Table.sql";
            public static string Files => "dbo.Files.Table.sql";
            public static string Permissions => "dbo.Permissions.Table.sql";
            public static string RolePermissions => "dbo.RolePermissions.Table.sql";
            public static string AuditLogs => "dbo.AuditLogs.Table.sql";

            public static string SP_GetUsersFiltered = "dbo.GetUsersFiltered.StoredProcedure.sql";

            public static string SCRIPT_Seed = "dbo.Seed.Script.sql";
        }
    }
}
