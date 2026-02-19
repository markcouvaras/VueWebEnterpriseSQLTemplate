using System.Net.Sockets;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using VueWebEnterpriseSQL.Infrastructure.Data;
using VueWebEnterpriseSQL.Infrastructure.Messaging;
using VueWebEnterpriseSQL.Infrastructure.Queries;
using VueWebEnterpriseSQL.Application.Interfaces;
using VueWebEnterpriseSQL.Infrastructure.Services;

namespace VueWebEnterpriseSQL.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Returns true if the connection string is configured and SQL Server is reachable.
        /// Uses a quick TCP check so the app can start without external dependencies.
        /// </summary>
        private static bool IsSqlServerAvailable(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)
                || connectionString.Contains("PLEASE_CONFIGURE_ME", StringComparison.OrdinalIgnoreCase))
                return false;

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                var host = builder.DataSource;

                // Parse host and port (supports "host,port" and "host" formats)
                var parts = host.Split(',');
                var server = parts[0].Trim();
                var port = parts.Length > 1 && int.TryParse(parts[1].Trim(), out var p) ? p : 1433;

                using var client = new TcpClient();
                var result = client.BeginConnect(server, port, null, null);
                var connected = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                if (connected) client.EndConnect(result);
                return connected;
            }
            catch
            {
                return false;
            }
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var sqlAvailable = IsSqlServerAvailable(connectionString);

            // 1. Database Configuration (with retry policy for resilience)
            // EF Core connections are lazy — this won't fail at startup even if SQL is down.
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
                .AddDbContextCheck<AppDbContext>(name: "database", tags: new[] { "db", "ready" });

            if (sqlAvailable)
            {
                healthChecks.AddSqlServer(connectionString!, name: "sqlserver", tags: new[] { "db", "ready" });
            }

            if (!string.IsNullOrEmpty(redisConnection))
            {
                healthChecks.AddRedis(redisConnection, name: "redis", tags: new[] { "cache", "ready" });
            }

            // 7. Email Services (MailKit + Hangfire)
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddScoped<IEmailManager, EmailManager>();

            // --- 8. HANGFIRE SETUP (Start) ---
            // Hangfire requires a working SQL Server connection to install its schema at
            // startup. Only configure when the database is reachable.
            // Run 'docker-compose up -d' to start SQL Server, Redis, and MailHog.
            if (sqlAvailable)
            {
                services.AddHangfire(config => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString));

                // Add the processing server (The thing that actually runs the jobs)
                services.AddHangfireServer();
            }
            // --- HANGFIRE SETUP (End) ---

            return services;
        }
    }
}
