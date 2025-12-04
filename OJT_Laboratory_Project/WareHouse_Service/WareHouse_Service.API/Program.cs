using MediatR;
using WareHouse_Service.API.GrpcServer;
using WareHouse_Service.Application.Instruments.Commands;
using WareHouse_Service.Infrastructure;

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

// -----------------------------------------------------
// 1️⃣ Add services to DI
// -----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WareHouse Service API",
        Version = "v1",
        Description = "API for warehouse and instrument management"
    });
});

// Register infrastructure services (DbContext, Repositories, gRPC Clients)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 1.d MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ProcessTestOrderCommandHandler).Assembly));

// 1.e gRPC Server
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6 * 1024 * 1024;
    options.MaxSendMessageSize = 6 * 1024 * 1024;
});

// -----------------------------------------------------
// 2️⃣ Build app
// -----------------------------------------------------
var app = builder.Build();

// -----------------------------------------------------
// 3️⃣ Middleware
// -----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend"); // Must be before UseAuthentication and UseAuthorization

app.UseRouting();
app.UseAuthorization();

// Map Controllers, gRPC Services, and Health Checks (top-level route registration)
app.MapControllers();
app.MapGrpcService<WareHouse_Service.API.GrpcServer.WareHouse_Service>();
app.MapHealthChecks("/health");

// -----------------------------------------------------
// 4️⃣ Run app
// -----------------------------------------------------
app.Run();
