using AuthWithStorage.Application.Mappers;
using AuthWithStorage.Application.Validators;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Infrastructure.Data;
using AuthWithStorage.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.IdentityModel.Protocols.Configuration;
using System;
using System.Text;
using AuthWithStorage.Application.DTOs;
using AuthWithStorage.Domain.Queries;

namespace AuthWithStorage.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InitServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddOpenAPI()
                .AddDatabase(configuration)
                .AddAutoMapper(typeof(MappingProfile))
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
            services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
            services.AddScoped<IRepository<User, int, UserSearchQuery>, UserRepository>();
            return services;
        }

            return services;
        }
    }
}
