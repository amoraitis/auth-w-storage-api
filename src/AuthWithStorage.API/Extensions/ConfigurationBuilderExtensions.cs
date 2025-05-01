namespace AuthWithStorage.API.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
            public static IConfigurationBuilder RegisterConfiguration(this IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
            {
                configurationBuilder.Sources.Clear();
                return configurationBuilder
                    .ConfigureEnvironmentVariables()
                    .ConfigureJsonProvider(hostEnvironment);
            }

            private static IConfigurationBuilder ConfigureEnvironmentVariables(this IConfigurationBuilder configurationBuilder)
            {
                configurationBuilder.AddEnvironmentVariables(prefix: "AuthWithStorage_");

                return configurationBuilder;
            }

            private static IConfigurationBuilder ConfigureJsonProvider(this IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
            {
                configurationBuilder
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json",
                        optional: true, reloadOnChange: true);

                return configurationBuilder;
            }
    }
}
