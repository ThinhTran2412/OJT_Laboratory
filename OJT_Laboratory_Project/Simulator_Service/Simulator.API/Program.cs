using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Simulator.API.Service;
using Simulator.Application.HostedService;
using Simulator.Application.Interface;
using Simulator.Application.SimulateRawData.Command;
using Simulator.Infastructure.Data;
using Simulator.Infastructure.RabbitMQ;
using Simulator.Infastructure.Repository;
using MediatR;
using System.Reflection;
using System.Linq;

// CRITICAL: Enable HTTP/2 unencrypted support for gRPC inter-service communication in Docker
// This must be set BEFORE creating the WebApplication builder
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

// Add Health Checks
builder.Services.AddHealthChecks();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Simulator Service API",
        Version = "v1",
        Description = "API for simulating laboratory test data"
    });
});

// Add gRPC services
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6 * 1024 * 1024; // 6 MB
    options.MaxSendMessageSize = 6 * 1024 * 1024;    // 6 MB
});

// Add MediatR for CQRS pattern
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssembly(typeof(SimulateRawDataCommandHandler).Assembly);
});

// Add gRPC Service
builder.Services.AddScoped<RawDataQueryGrpcService>();

// --- Configure RabbitMQ ---
builder.Services.Configure<Simulator.Infastructure.RabbitMQ.RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Factory Connection là Singleton
builder.Services.AddSingleton<IConnection>(sp =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<Simulator.Infastructure.RabbitMQ.RabbitMQSettings>>().Value;
    return Simulator.Infastructure.RabbitMQ.RabbitMQConfig.GetConnection(settings);
});

// Publisher là Singleton
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

// --- Repository and DB Context ---
// Đăng ký Interface cho Repository
builder.Services.AddScoped<IRawTestResultRepository, RawTestResultRepository>();

// Đăng ký DbContext
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    // Ưu tiên lấy từ biến môi trường DATABASE_URL (Render, Railway, v.v.)
    var envConnection = Environment.GetEnvironmentVariable("DATABASE_URL");

    // Nếu không có thì lấy trong appsettings.json
    var connectionString = !string.IsNullOrEmpty(envConnection)
        ? ConvertPostgresUrlToConnectionString(envConnection)
        : config.GetConnectionString("DefaultConnection");

    // Lấy schema từ appsettings.json, mặc định là "simulator_service"
    var schema = config.GetValue<string>("Database:Schema") ?? "simulator_service";

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", schema);
    });
});

// Add Hosted Service
builder.Services.AddHostedService<Simulator.Application.HostedService.RawDataSimulationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseCors("AllowFrontend"); // Must be before UseAuthentication and UseAuthorization

// Map Controllers, gRPC Services, and Health Checks
app.MapControllers();
app.MapGrpcService<RawDataQueryGrpcService>();
app.MapHealthChecks("/health");

app.Run();

// Helper function to convert PostgreSQL URL to connection string
static string ConvertPostgresUrlToConnectionString(string postgresUrl)
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
