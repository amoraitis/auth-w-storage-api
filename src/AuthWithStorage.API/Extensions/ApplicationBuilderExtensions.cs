namespace AuthWithStorage.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplicationBuilder InitApp(this WebApplicationBuilder app)
        {
            app.Configuration.RegisterConfiguration(app.Environment);

            return app;
        }
    }
}