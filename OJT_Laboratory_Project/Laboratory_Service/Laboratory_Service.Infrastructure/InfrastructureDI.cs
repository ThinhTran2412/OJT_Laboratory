using Infrastructure.Repositories;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.Services;
using Laboratory_Service.Infrastructure.Data;
using Laboratory_Service.Infrastructure.GrpcClient;
using Laboratory_Service.Infrastructure.GrpcClients;
using Laboratory_Service.Infrastructure.RabbitMQ;
using Laboratory_Service.Infrastructure.Repositories;
using Laboratory_Service.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Laboratory_Service.Infrastructure
{
    public static class InfrastructureDI
    {
        /// <summary>
        /// Registers all infrastructure-level services such as DbContext, repositories, and authentication providers.
        /// Called from Program.cs in the API layer.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // --- 1. Database Context Registration ---
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                // Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
                var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Nếu không có thì lấy trong appsettings.json
                var connectionString = !string.IsNullOrEmpty(envConnection)
                    ? ConvertPostgresUrlToConnectionString(envConnection)
                    : config.GetConnectionString("DefaultConnection");

                // Lấy schema từ appsettings.json, mặc định là "laboratory_service"
                var schemaName = config["Database:Schema"] ?? "laboratory_service";
                
                // Set schema vào static property của AppDbContext
                Data.AppDbContext.SchemaName = schemaName;

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Cấu hình migration history table vào schema riêng
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schemaName);
                });
            });

            services.AddGrpcClient<IAM_Service.API.Protos.UserService.UserServiceClient>(options =>
            {
                // On Render, use HTTPS public URLs for inter-service gRPC
                // Priority: HTTPS URL from config file > Environment Variable > Default
                // NOTE: If env var contains private URL (http://iam-service:8080), it will fail with "Name or service not known"
                // Solution: Delete IAM_SERVICE_GRPC_URL env var in Render Dashboard or set it to HTTPS URL
                var envGrpcUrl = Environment.GetEnvironmentVariable("IAM_SERVICE_GRPC_URL");
                var configGrpcUrl = configuration["IAMService:GrpcUrl"];
                
                // Prefer HTTPS URL from config file (production), only use env var if config is missing
                string grpcUrl;
                if (!string.IsNullOrEmpty(configGrpcUrl) && configGrpcUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // Config has HTTPS URL - use it (production)
                    grpcUrl = configGrpcUrl;
                }
                else if (!string.IsNullOrEmpty(envGrpcUrl))
                {
                    // Use env var if config doesn't have HTTPS URL
                    grpcUrl = envGrpcUrl;
                }
                else
                {
                    // Fallback to config or default
                    grpcUrl = configGrpcUrl ?? "http://localhost:7001";
                }
                
                // Log configuration to console for debugging (will appear in Render logs)
                Console.WriteLine($"[gRPC Config] IAM Service gRPC URL Configuration:");
                Console.WriteLine($"[gRPC Config]   - Environment Variable (IAM_SERVICE_GRPC_URL): {envGrpcUrl ?? "(not set)"}");
                Console.WriteLine($"[gRPC Config]   - Configuration (IAMService:GrpcUrl): {configGrpcUrl ?? "(not set)"}");
                Console.WriteLine($"[gRPC Config]   - Final gRPC URL: {grpcUrl}");
                Console.WriteLine($"[gRPC Config]   - Is HTTPS: {grpcUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)}");
                if (!string.IsNullOrEmpty(envGrpcUrl) && !envGrpcUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[gRPC Config]   - WARNING: Environment variable has non-HTTPS URL. This may cause 'Name or service not known' error.");
                    Console.WriteLine($"[gRPC Config]   - WARNING: Delete IAM_SERVICE_GRPC_URL env var in Render Dashboard to use HTTPS URL from config file.");
                }
                
                options.Address = new Uri(grpcUrl);
            }).ConfigureChannel(options =>
            {
                var envGrpcUrl = Environment.GetEnvironmentVariable("IAM_SERVICE_GRPC_URL");
                var configGrpcUrl = configuration["IAMService:GrpcUrl"];
                
                // Prefer HTTPS URL from config file (production), only use env var if config is missing
                string grpcUrl;
                if (!string.IsNullOrEmpty(configGrpcUrl) && configGrpcUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    grpcUrl = configGrpcUrl;
                }
                else if (!string.IsNullOrEmpty(envGrpcUrl))
                {
                    grpcUrl = envGrpcUrl;
                }
                else
                {
                    grpcUrl = configGrpcUrl ?? "http://localhost:7001";
                }
                var isHttps = !string.IsNullOrEmpty(grpcUrl) && grpcUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                
                // CRITICAL: Enable HTTP/2 unencrypted support for Docker inter-service communication
                // This must be set BEFORE creating the HttpHandler
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                
                var httpHandler = new System.Net.Http.SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    ConnectTimeout = TimeSpan.FromSeconds(30) // Connection timeout
                };

                // On Render, use HTTPS public URLs for gRPC inter-service communication
                // Render load balancer handles SSL termination and forwards with HTTP/2 if server supports it
                // gRPC over HTTPS will automatically negotiate HTTP/2 via ALPN (Application-Layer Protocol Negotiation)
                if (isHttps)
                {
                    // For HTTPS, trust server certificate (Render load balancer certificate)
                    httpHandler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    };
                }

                options.HttpHandler = httpHandler;
                // Set max send/receive message size
                options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
                options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
            });

            services.AddGrpcClient<WareHouse_Service.API.Protos.WarehouseService.WarehouseServiceClient>(options =>
            {
                var grpcUrl = configuration["WareHouseService:GrpcUrl"] ?? "http://localhost:7002";
                options.Address = new Uri(grpcUrl);
            }).ConfigureChannel(options =>
            {
                // Enable HTTP/2 unencrypted support for Docker inter-service communication
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                
                options.HttpHandler = new System.Net.Http.SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan
                };
            });

            services.AddHostedService<RabbitMQRawResultConsumer>();
            // --- 2. Repository Registrations ---
            services.AddScoped<IRawBackupRepository, RawBackupRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
            services.AddScoped<ITestOrderRepository, TestOrderRepository>();
            services.AddScoped<ITestResultRepository, TestResultRepository>();
            services.AddScoped<IFlaggingConfigRepository, FlaggingConfigRepository>();
            services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
            services.AddScoped<IPatientService,PatientService>();


            services.AddScoped<IIamUserService, IamUserGrpcClientService>();
            services.AddScoped<IWareHouseGrpcClient, WareHouseGrpcClientService>();
            services.AddScoped<IEventLogService, EventLogRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IEncryptionService, DeterministicAesEncryptionService>();
            services.AddSingleton<IAiReviewService, AiReviewService>();

            return services;
        }
        
        private static string ConvertPostgresUrlToConnectionString(string url)
        {
            // URL kiểu: postgresql://user:pass@host:port/db hoặc postgresql://user:pass@host/db
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";

            // Nếu không có port trong URL, dùng port mặc định 5432 cho PostgreSQL
            var port = uri.Port == -1 ? 5432 : uri.Port;

            return $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={username};Password={password};Ssl Mode=Require;Trust Server Certificate=true";
        }
    }
}
