using AuthWithStorage.API.Extensions;
using AuthWithStorage.Infrastructure.Data;

namespace AuthWithStorage.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication
                .CreateBuilder(args)
                .InitApp();

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.InitServices(builder.Configuration);

            var app = builder.Build();

            InitDatabase(builder.Configuration);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void InitDatabase(ConfigurationManager configuration)
        {
            // Ensure database is created and migrations are applied
            var sysConnectionString = configuration["SysConnectionString"];
            var appConnectionString = configuration["ConnectionString"];
            Microsoft.Data.SqlClient.SqlConnectionStringBuilder csBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(appConnectionString);

            string database = csBuilder.InitialCatalog;
            Database.EnsureCreated(sysConnectionString, database);
            Database.EnsureMigrations(appConnectionString, configuration);
        }
    }
}