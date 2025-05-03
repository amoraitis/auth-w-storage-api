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
                SqlScriptsConfig.AuditLogs
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
        }
    }
}
