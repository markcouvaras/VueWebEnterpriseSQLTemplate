using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VueWebEnterpriseSQL.Api.Extensions
{
    /// <summary>
    /// Configures Swagger to generate a separate document per API version.
    /// </summary>
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithVersioning(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
            });

            return services;
        }

        public static WebApplication UseSwaggerWithVersioning(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Build a Swagger endpoint for each discovered API version
                var descriptions = app.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });

            return app;
        }
    }

    /// <summary>
    /// Dynamically creates a Swagger document for each discovered API version.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "VueWebEnterpriseSQL API",
                Version = description.ApiVersion.ToString(),
                Description = "Clean Architecture API with versioning."
            };

            if (description.IsDeprecated)
            {
                info.Description += " [DEPRECATED]";
            }

            return info;
        }
    }

    /// <summary>
    /// Fixes Swagger UI parameters for versioned endpoints.
    /// Marks the version parameter as required and sets the default value.
    /// Compatible with Microsoft.OpenApi v2+ (Swashbuckle 10.x).
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = context.ApiDescription.ParameterDescriptions
                    .First(p => p.Name == parameter.Name);

                if (parameter is OpenApiParameter concreteParam)
                {
                    concreteParam.Description ??= description.ModelMetadata?.Description;
                    concreteParam.Required |= description.IsRequired;
                }
            }
        }
    }
}
