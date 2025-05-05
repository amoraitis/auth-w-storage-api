using AuthWithStorage.API.Extensions;
using AuthWithStorage.Infrastructure.Data;
using Serilog;

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
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();
#pragma warning disable ASP0014 // Suggest using top level route registrations
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization().RequireRateLimiting("fixed");
                endpoints.MapHealthChecks("/health");
            });
#pragma warning restore ASP0014 // Suggest using top level route registrations

            try
            {
                Log.Information("Starting up");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
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