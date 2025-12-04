using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WareHouse_Service.Application.Interface;
using WareHouse_Service.Infrastructure.Data;
using WareHouse_Service.Infrastructure.Repository;
using WareHouse_Service.Infrastructure.GrpcClient;
using Simulator.API.Protos.Query;
using Grpc.Net.ClientFactory;

namespace WareHouse_Service.Infrastructure
{
    public static class InfrastructureDI
    {
        /// <summary>
        /// Registers all infrastructure-level services such as DbContext, repositories, and gRPC clients.
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

                // Lấy schema từ appsettings.json, mặc định là "warehouse_service"
                var schemaName = config["Database:Schema"] ?? "warehouse_service";
                
                // Set schema vào static property của AppDbContext
                Data.AppDbContext.SchemaName = schemaName;

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Cấu hình migration history table vào schema riêng
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schemaName);
                });
            });

            // --- 2. Repository Registrations ---
            services.AddScoped<IInstrumentRepository, InstrumentRepository>();

            // --- 3. gRPC Client (Simulator) ---
            var simulatorUrl = configuration["GrpcSettings:SimulatorServiceUrl"] 
                ?? "http://simulator-service:8081";

            services.AddGrpcClient<RawDataQueryService.RawDataQueryServiceClient>(options =>
            {
                options.Address = new Uri(simulatorUrl);
            }).ConfigureChannel(options =>
            {
                // Enable HTTP/2 unencrypted support (Docker internal)
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                options.HttpHandler = new System.Net.Http.SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    ConnectTimeout = TimeSpan.FromSeconds(30)
                };
                options.MaxReceiveMessageSize = 6 * 1024 * 1024;
                options.MaxSendMessageSize = 6 * 1024 * 1024;
            });

            // --- 4. gRPC Client Wrapper ---
            services.AddScoped<ISimulatorGrpcClient, SimulatorGrpcClient>();

            return services;
        }

        /// <summary>
        /// Helper function to convert PostgreSQL URL to connection string
        /// </summary>
        private static string ConvertPostgresUrlToConnectionString(string postgresUrl)
        {
            if (string.IsNullOrWhiteSpace(postgresUrl))
                return string.Empty;

            var uri = new Uri(postgresUrl);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";

            // Nếu không có port trong URL, dùng port mặc định 5432 cho PostgreSQL
            var port = uri.Port == -1 ? 5432 : uri.Port;

            var connectionString = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={username};Password={password}";

            // Add SSL mode for external connections
            if (uri.Host.Contains(".render.com") || uri.Host.Contains(".railway.app"))
            {
                connectionString += ";SSL Mode=Require;Trust Server Certificate=true";
            }

            return connectionString;
        }
    }
}

