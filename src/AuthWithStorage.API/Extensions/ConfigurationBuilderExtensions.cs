using Microsoft.Extensions.FileProviders;

namespace AuthWithStorage.API.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
            public static IConfigurationBuilder RegisterConfiguration(this IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
            {
                configurationBuilder.Sources.Clear();
                return configurationBuilder
                    .ConfigureEnvironmentVariables()
                    .ConfigureJsonProvider(hostEnvironment)
                    .ConfigureSQLScripts();
            }

            private static IConfigurationBuilder ConfigureEnvironmentVariables(this IConfigurationBuilder configurationBuilder)
            {
                configurationBuilder.AddEnvironmentVariables(prefix: "AuthAPIWithStorage_");

                return configurationBuilder;
            }

            private static IConfigurationBuilder ConfigureJsonProvider(this IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
            {
                configurationBuilder
                    .SetBasePath(hostEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json",
                        optional: true, reloadOnChange: true);

                return configurationBuilder;
            }

             private static IConfigurationBuilder ConfigureSQLScripts(this IConfigurationBuilder configurationBuilder)
             {
                 configurationBuilder.AddKeyPerFile(
                     optional: false,
                     directoryPath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sql"));

                return configurationBuilder;
            }
    }
}
