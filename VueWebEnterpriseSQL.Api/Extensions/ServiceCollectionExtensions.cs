using Asp.Versioning;
using VueWebEnterpriseSQL.Application;
using VueWebEnterpriseSQL.Infrastructure;

namespace VueWebEnterpriseSQL.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Add Layers
            services.AddApplication();
            services.AddInfrastructure(configuration);

            // 2. Add API specific stuff
            services.AddExceptionHandler<Infrastructure.GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddHttpContextAccessor();

            // 3. Add API Versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // 4. Add CORS — read allowed origins from appsettings.json
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
            });

            return services;
        }
    }
}
