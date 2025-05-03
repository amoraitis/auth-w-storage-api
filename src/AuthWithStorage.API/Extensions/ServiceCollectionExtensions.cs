using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Infrastructure.Data;
using AuthWithStorage.Infrastructure.Repositories;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace AuthWithStorage.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InitServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddOpenAPI()
                .AddDatabase(configuration)
                .AddRepositories();
        }

        private static IServiceCollection AddOpenAPI(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "AuthWithStorage API",
                    Version = "v1"
                });
            });
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<DbContext>(new DbContext(configuration["ConnectionString"] ?? throw new InvalidConfigurationException("Missing connectionString!")));
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User, int>, UserRepository>();
            return services;
        }
    }
}
