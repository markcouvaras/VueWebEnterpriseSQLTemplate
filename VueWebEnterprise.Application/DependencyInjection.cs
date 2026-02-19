using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using VueWebEnterprise.Application.Common.Behaviours;

namespace VueWebEnterprise.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 1. Register Validators (The Rule Book)
            // This scans the assembly for anything inheriting from AbstractValidator<T>
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // 2. Register MediatR (The Traffic Cop)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                // Register the Logging Pipeline (runs first — logs start/end/errors)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
                // Register the Validation Pipeline (runs second — validates before handler)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });

            return services;
        }
    }
}
