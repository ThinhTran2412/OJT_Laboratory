using Laboratory_Service.Application;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Services;
using Laboratory_Service.Infrastructure;
using Laboratory_Service.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyCompany.Authorization.Setup;
using SharedLibrary.DependencyInjection;
using System.Linq;

namespace Laboratory_Service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a builder for the web application.
            var builder = WebApplication.CreateBuilder(args);

            // CRITICAL: Enable HTTP/2 unencrypted support for gRPC inter-service communication in Docker
            // This must be set BEFORE any gRPC clients are configured
            // Enable for both CLIENT (HTTP client) and SERVER (Kestrel) side
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            // Also set via environment variable check (for server-side Kestrel)
            var http2UnencryptedSupport = Environment.GetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2UNENCRYPTEDSUPPORT");
            if (string.IsNullOrEmpty(http2UnencryptedSupport))
            {
                Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2UNENCRYPTEDSUPPORT", "true");
            }

            // Configure Kestrel for Production FIRST - before any other services
            // Disable HTTPS in production - Render handles HTTPS at the load balancer level
            if (builder.Environment.IsProduction())
            {
                // Clear ASPNETCORE_URLS to prevent conflicts with UseKestrel
                Environment.SetEnvironmentVariable("ASPNETCORE_URLS", null);
                
                // Remove ALL Kestrel endpoint configurations from appsettings.json
                // This prevents conflicts when UseKestrel creates new endpoints
                var kestrelSection = builder.Configuration.GetSection("Kestrel");
                if (kestrelSection.Exists())
                {
                    // Remove all child keys from Kestrel section to completely remove it
                    foreach (var child in kestrelSection.GetChildren().ToList())
                    {
                        if (child.Key == "Endpoints")
                        {
                            foreach (var endpoint in child.GetChildren().ToList())
                            {
                                foreach (var endpointChild in endpoint.GetChildren().ToList())
                                {
                                    builder.Configuration[$"Kestrel:Endpoints:{endpoint.Key}:{endpointChild.Key}"] = null;
                                }
                                builder.Configuration[$"Kestrel:Endpoints:{endpoint.Key}"] = null;
                            }
                            builder.Configuration["Kestrel:Endpoints"] = null;
                        }
                        else
                        {
                            builder.Configuration[$"Kestrel:{child.Key}"] = null;
                        }
                    }
                    builder.Configuration["Kestrel"] = null;
                }
                
                // Use UseKestrel to completely replace configuration
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
                
                // Disable HTTPS redirection in production
                builder.Services.Configure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
                {
                    options.RedirectStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = null; // Disable HTTPS redirection
                });
            }

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Laboratory Service API",
                    Version = "v1",
                    Description = "API for managing patients and medical records in Laboratory Service"
                });
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

            // Add Health Checks
            builder.Services.AddHealthChecks();

            // Add gRPC services
            builder.Services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });

            builder.Services.AddSingleton<ITokenService, IAMTokenService>();

            builder.Services.AddHttpClient<IIAMService, IAMService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["IAMService:BaseUrl"]!);
                client.Timeout = TimeSpan.FromSeconds(30); // 30 second timeout for IAM Service calls
            });
            // Add application services
            builder.Services.AddApplication(builder.Configuration);

            // Add infrastructure services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Add shared services
            builder.Services.AddSharedServices(builder.Configuration);
            builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
            // builder.Services.Configure<LockoutOptions>(builder.Configuration.GetSection("Lockout"));
            builder.Services.AddPrivilegePolicies();


            var app = builder.Build();

            // Apply database migrations automatically (only in Production when migrations table doesn't exist)
            // This is safe because migrations are idempotent
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.AppDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    
                    // Check if migrations need to be applied
                    var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying {Count} pending database migrations...", pendingMigrations.Count);
                        foreach (var migration in pendingMigrations)
                        {
                            logger.LogInformation("Pending migration: {Migration}", migration);
                        }
                        
                        dbContext.Database.Migrate();
                        logger.LogInformation("Database migrations applied successfully");
                    }
                    else
                    {
                        logger.LogInformation("Database is up to date - no pending migrations");
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Failed to apply database migrations. Service will continue, but some features may not work correctly.");
                // Don't throw - allow service to start even if migrations fail
                // This prevents the entire service from crashing due to migration issues
            }

            // CORS middleware - MUST be first, before any other middleware
            // This ensures CORS headers are added to all responses including preflight OPTIONS requests
            app.UseCors("AllowFrontend");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Exception handler for production - must preserve CORS headers in error responses
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                        var exception = exceptionHandlerPathFeature?.Error;
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        
                        // Log the exception with full details
                        if (exception != null)
                        {
                            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                        }
                        else
                        {
                            logger.LogError("Unknown error occurred");
                        }
                        
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        
                        // In production, return generic message but log details
                        // CORS headers are already added by UseCors middleware above
                        var response = new { message = "An error occurred while processing your request." };
                        await context.Response.WriteAsJsonAsync(response);
                    });
                });
            }

            app.UseSharePolicies();
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers, gRPC Services, and Health Checks
            app.MapControllers();
            app.MapGrpcService<Laboratory_Service.API.Services.PatientGrpcService>();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}