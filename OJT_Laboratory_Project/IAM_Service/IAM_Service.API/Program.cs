using IAM_Service.Application;
using IAM_Service.Infrastructure;
using SharedLibrary.DependencyInjection;
using IAM_Service.Application.Common.Security;
using IAM_Service.API.Services;
using MyCompany.Authorization.Setup;
using System.Linq;


namespace IAM_Service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CRITICAL: Enable HTTP/2 unencrypted support for gRPC inter-service communication in Docker
            // This must be set BEFORE creating the WebApplication builder
            // Enable for both CLIENT (HTTP client) and SERVER (Kestrel) side
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            // Also set via environment variable check (for server-side Kestrel)
            var http2UnencryptedSupport = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2UNENCRYPTEDSUPPORT");
            if (string.IsNullOrEmpty(http2UnencryptedSupport))
            {
                Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2UNENCRYPTEDSUPPORT", "true");
            }
            
            var builder = WebApplication.CreateBuilder(args);

            // Configure Kestrel for Docker environment (both Development and Production)
            // Disable HTTPS in Docker - Nginx handles HTTPS at the reverse proxy level
            // Always configure Kestrel explicitly when running in Docker
            // Clear ASPNETCORE_URLS to prevent conflicts with UseKestrel
            Environment.SetEnvironmentVariable("ASPNETCORE_URLS", null);
            
            // Use UseKestrel to completely replace ALL configuration from appsettings
            // This will override any Kestrel settings in appsettings.json files
            builder.WebHost.UseKestrel(options =>
            {
                // Configure HTTP endpoints - UseKestrel will override all existing config
                var port = Environment.GetEnvironmentVariable("PORT");
                var portNumber = !string.IsNullOrEmpty(port) ? int.Parse(port) : 8080;
                var grpcPort = portNumber + 1; // gRPC port = REST port + 1 (8081)
                
                // REST API endpoint - HTTP/1.1 only (for Nginx proxy)
                options.ListenAnyIP(portNumber, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                });
                
                // gRPC endpoint - HTTP/2 only (unencrypted, for Docker internal network)
                // Services communicate directly via Docker network, not through Nginx
                options.ListenAnyIP(grpcPort, listenOptions =>
                {
                    // HTTP/2 unencrypted support is enabled via AppContext.SetSwitch above
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });
            
            // Disable HTTPS redirection in Docker
            builder.Services.Configure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
            {
                options.RedirectStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = null; // Disable HTTPS redirection
            });

            // Configure CORS
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "https://front-end-fnfs.onrender.com", "http://localhost:5173", "http://localhost:3000" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add controllers and Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "IAM Service API",
                    Version = "v1",
                    Description = "API for Identity and Access Management"
                });

                // Include all XML comments if available
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            // Add Health Checks
            builder.Services.AddHealthChecks();

            // Add gRPC (with detailed errors and message size limits)
            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });

            // Register internal gRPC services
            builder.Services.AddScoped<UserGrpcService>();
            //builder.Services.AddSingleton<PatientGrpcClientService>();

            // Register application and infrastructure layers
            builder.Services.AddApplication();
            builder.Services.AddAutoMapper(typeof(IAM_Service.Application.Common.Mappings.AutoMapperProfile));

            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddSharedServices(builder.Configuration);
            builder.Services.AddPrivilegePolicies();

            builder.Services.Configure<LockoutOptions>(builder.Configuration.GetSection("Lockout"));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middlewares
            app.UseCors("AllowFrontend"); // Must be before UseAuthentication and UseAuthorization
            app.UseSharePolicies();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers, gRPC Services, and Health Checks
            app.MapControllers();
            app.MapGrpcService<UserGrpcService>();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
