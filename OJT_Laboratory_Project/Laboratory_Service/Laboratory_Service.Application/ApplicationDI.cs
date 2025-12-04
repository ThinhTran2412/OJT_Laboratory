using System;
using System.Reflection;
using FluentValidation;
using Laboratory_Service.Application.Common.Behavior;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Laboratory_Service.Application
{
    public static class ApplicationDI
    {
        /// <summary>
        /// Extends the IServiceCollection to register all services required by the Application Layer.
        /// This method should be called from the presentation layer's Program.cs/Startup.cs.
        /// </summary>
        /// <param name="services">The service collection used for DI registration.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
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
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // --- 3. Application Services Registration ---

            // Register IAM Service
            services.AddHttpClient<IIAMService, IAMService>(client =>
            {
                string? iamBaseUrl = configuration["IAMService:BaseUrl"];

                if (string.IsNullOrEmpty(iamBaseUrl))
                {
                    throw new InvalidOperationException("Thiếu cấu hình: 'IAMService:BaseUrl' không được tìm thấy. Vui lòng kiểm tra appsettings.json.");
                }
                client.BaseAddress = new Uri(iamBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30); // 30 second timeout for IAM Service calls
            });

            // Register Patient application service (use-cases)
            services.AddScoped<IPatientAppService, PatientAppService>();

            // Register FlaggingService
            services.AddScoped<IFlaggingService, FlaggingService>();

            return services;
        }
    }
}
