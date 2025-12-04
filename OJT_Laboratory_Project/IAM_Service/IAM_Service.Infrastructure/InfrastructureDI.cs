using IAM_Service.Application.Interface.IAuditLogRepository;
using IAM_Service.Application.Interface.IClient;
using IAM_Service.Application.Interface.IEmailSender;
using IAM_Service.Application.Interface.IEncryptionService;
using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IPasswordResetRepository;
using IAM_Service.Application.Interface.IPrivilege;
using IAM_Service.Application.Interface.IRefreshToken;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Infrastructure.Authentication;
using IAM_Service.Infrastructure.Data;
using IAM_Service.Infrastructure.EmailSenders;
using IAM_Service.Infrastructure.Interceptor;
using IAM_Service.Infrastructure.Repositories;
using IAM_Service.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAM_Service.Infrastructure
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
            services.AddScoped<AuditLogInterceptor>();
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditLogInterceptor>();
                var config = sp.GetRequiredService<IConfiguration>();

                // Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
                var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Nếu không có thì lấy trong appsettings.json
                var connectionString = !string.IsNullOrEmpty(envConnection)
                    ? ConvertPostgresUrlToConnectionString(envConnection)
                    : config.GetConnectionString("DefaultConnection");

                // Lấy schema từ appsettings.json, mặc định là "iam_service"
                var schemaName = config["Database:Schema"] ?? "iam_service";
                
                // Set schema vào static property của AppDbContext
                Data.AppDbContext.SchemaName = schemaName;

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Cấu hình migration history table vào schema riêng
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schemaName);
                })
                    .AddInterceptors(interceptor);
            });

            // --- 2. Repository Registrations ---
            // Query Repositories (read)
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            // User repositories
            var userRepo = typeof(UserRepository);
            services.AddScoped<IUsersRepository, UserRepository>();

            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            // Command Repositories (write)
            services.AddScoped<IRoleCommandRepository, RoleCommandRepository>();

            // Register additional services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IEncryptionService, DeterministicAesEncryptionService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
            services.AddScoped<IClientRepository, ClientRepository>();


            // Register JWT provider and configuration
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
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
