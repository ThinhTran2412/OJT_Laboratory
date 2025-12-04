using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Middleware;

namespace SharedLibrary.DependencyInjection
{
    /// <summary>
    /// The Extension Class is used to register and configure 
    /// services and middleware shared by the entire system. 
    /// Specifically, this class is responsible for: 
    /// - Registering JWT authentication configurations (AddJWTAuthenticationScheme) 
    /// - Registering global error handling middleware (GlobalExceptionHandler) 
    /// - (Optional) Blocking external requests that do not go through the API Gateway
    /// </summary>
    public static class ShareServiceContainer
    {
        /// <summary>
        /// Adds the shared services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration config)
        {
            // JWT authentication disabled
            // JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);
            return services;
        }

        /// <summary>
        /// Uses the share policies.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseSharePolicies(this IApplicationBuilder app)
        {
            // Use global exception
           // app.UseMiddleware<GlobalExceptionHandler>();

            // Middle to block all outsiders api calls

            //app.UseMiddleware<ListenToOnlyApiGateway>();


            return app;
        }
    }
}
