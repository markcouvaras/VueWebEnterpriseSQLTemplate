using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using VueWebEnterprise.Infrastructure.Data;
using VueWebEnterprise.Infrastructure.Messaging;
using VueWebEnterprise.Infrastructure.Queries;
using VueWebEnterprise.Application.Interfaces;
using VueWebEnterprise.Infrastructure.Services;

namespace VueWebEnterprise.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 1. Database Configuration (with retry policy for resilience)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)));

            // 2. Distributed Cache (Cloud-Agnostic)
            var redisConnection = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            // 3. Enable accessing the current HTTP Request (needed for Cookies/Headers)
            services.AddHttpContextAccessor();

            // 4. Register the User Service
            // "When a Controller asks for ICurrentUserService, give them CurrentUserService"
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // 5. Register Read Queries (Dapper — CQRS Read Side)
            services.AddScoped<IUserReadRepository, UserQuery>();

            // 6. Health Checks — verify infrastructure connectivity
            var healthChecks = services.AddHealthChecks()
                .AddDbContextCheck<AppDbContext>(name: "database", tags: new[] { "db", "ready" })
                .AddSqlServer(connectionString!, name: "sqlserver", tags: new[] { "db", "ready" });

            if (!string.IsNullOrEmpty(redisConnection))
            {
                healthChecks.AddRedis(redisConnection, name: "redis", tags: new[] { "cache", "ready" });
            }

            // 7. Email Services (MailKit + Hangfire)
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddScoped<IEmailManager, EmailManager>();

            // --- 8. HANGFIRE SETUP (Start) ---
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString));

            // Add the processing server (The thing that actually runs the jobs)
            services.AddHangfireServer();
            // --- HANGFIRE SETUP (End) ---

            return services;
        }
    }
}
