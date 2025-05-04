using Serilog;

namespace AuthWithStorage.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplicationBuilder InitApp(this WebApplicationBuilder app)
        {
            app.Configuration.RegisterConfiguration(app.Environment);
            return app.RegisterLogging();
        }

        public static WebApplicationBuilder RegisterLogging(this WebApplicationBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(app.Configuration)
                .CreateLogger();
            app.Host.UseSerilog();
            return app;
        }
    }
}