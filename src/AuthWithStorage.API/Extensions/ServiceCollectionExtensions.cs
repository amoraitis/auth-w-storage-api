using AuthWithStorage.Application.Mappers;
using AuthWithStorage.Application.Validators;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Infrastructure.Data;
using AuthWithStorage.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.Configuration;
using System;
using System.Text;
using AuthWithStorage.Application.DTOs;
using AuthWithStorage.Domain.Queries;
using AuthWithStorage.Infrastructure.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace AuthWithStorage.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InitServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddOpenAPI()
                .AddDatabase(configuration)
                .InitAuthentication(configuration)
                .InitJwtHandler(configuration)
                .AddAutoMapper(typeof(MappingProfile))
                .InitHealthChecks(configuration)
                .InitRateLimiting()
        }

        private static IServiceCollection InitAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PasswordOptions>(configuration.GetSection("PasswordOptions"));
            services.AddSingleton<PasswordGenerator>();
            services.AddSingleton<PasswordHasher>();
            services.AddSingleton<JwtService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    };
                });

            return services;
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

                // Add basic JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your token."
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
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

        private static IServiceCollection InitJwtHandler(this IServiceCollection services, IConfiguration configuration)
        {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var key = configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
            {
                throw new InvalidConfigurationException("JWT configuration is missing or invalid.");
            }

            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.Key = key;
                options.TokenExpiryInHours = int.TryParse(configuration["Jwt:TokenExpiryInHours"], out var expiry) ? expiry : 1;
            });

            services.AddSingleton<JwtService>();
            return services;
        }

        private static IServiceCollection InitHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddRedis(configuration.GetValue<string>("DataCache"))
                .AddSqlServer(
            connectionString: configuration.GetValue<string>("ConnectionString"),
            name: "sql",
            tags: new[] { "db", "sql" });

            return services;
        }

        private static IServiceCollection InitRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options =>
                {
                    options.PermitLimit = 4;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                }));
            return services;
        }

    }
}
