using FluentValidation;
using IAM_Service.Application.Common.Behavior;
using IAM_Service.Application.Helpers;
using IAM_Service.Application.Interface.IPrivilege;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IAM_Service.Application
{
    public static class ApplicationDI
    {
        /// <summary>
        /// Extends the IServiceCollection to register all services required by the Application Layer.
        /// This method should be called from the presentation layer's Program.cs/Startup.cs.
        /// </summary>
        /// <param name="services">The service collection used for DI registration.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // --- 1. MediatR Registration (CQRS Implementation) ---

            // Registers MediatR with the DI container.
            services.AddMediatR(cfg =>
            {
                // Scans the currently executing assembly (the Application layer assembly) 
                // to automatically find and register all Request Handlers (IRequestHandler), 
                // Notifications (INotificationHandler), and Pipelines.
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });


            // --- 2. AutoMapper Registration (DTO/Entity Mapping) ---

            // Registers AutoMapper with the DI container.
            // Scans the currently executing assembly (the Application layer assembly) 
            // to find all Profile classes where mapping definitions are configured.
            services.AddScoped<IPrivilegeNormalizationService, PrivilegeNormalizationService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


            return services;
        }
    }

}
