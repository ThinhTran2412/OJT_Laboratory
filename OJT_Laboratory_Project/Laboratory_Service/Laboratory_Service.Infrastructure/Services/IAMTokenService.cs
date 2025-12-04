using System.Text.Json;
using System.Text.Json.Serialization;
using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Infrastructure.Services
{
    /// <summary>
    /// Implementation để lấy và quản lý Access Token cho IAM Service (Client Credentials Flow).
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.ITokenService" />
    public class IAMTokenService : ITokenService
    {
        /// <summary>
        /// Implement methods form interface ITokenService
        /// </summary>
        private record TokenResponse(
            [property: JsonPropertyName("access_token")] string AccessToken,
            [property: JsonPropertyName("expires_in")] int ExpiresIn
        );

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<IAMTokenService> _logger;
        /// <summary>
        /// The token client
        /// </summary>
        private readonly HttpClient _tokenClient;

        /// <summary>
        /// The cached token
        /// </summary>
        private string? _cachedToken;
        /// <summary>
        /// The token expiration
        /// </summary>
        private DateTime _tokenExpiration = DateTime.MinValue.AddSeconds(30);

        /// <summary>
        /// Initializes a new instance of the <see cref="IAMTokenService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public IAMTokenService(IConfiguration configuration, ILogger<IAMTokenService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _tokenClient = new HttpClient(handler);
        }

        /// <summary>
        /// Gets the access token asynchronous.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// IAMService:ClientId is missing in configuration.
        /// or
        /// IAMService:ClientSecret is missing in configuration.
        /// </exception>
        public async Task<string?> GetAccessTokenAsync()
        {
            if (_cachedToken != null && _tokenExpiration > DateTime.UtcNow)
            {
                return _cachedToken;
            }

            _logger.LogInformation("Access token expired or not found. Requesting new token from IAM Service using Client Credentials.");

            var iamConfig = _configuration.GetSection("IAMService");
            var tokenEndpoint = $"{iamConfig["BaseUrl"]}{iamConfig["TokenEndpoint"]}";

            var clientId = iamConfig["ClientId"] ?? throw new InvalidOperationException("IAMService:ClientId is missing in configuration.");
            var clientSecret = iamConfig["ClientSecret"] ?? throw new InvalidOperationException("IAMService:ClientSecret is missing in configuration.");
            var scope = iamConfig["Scope"] ?? string.Empty;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", scope)
            });

            try
            {
                var response = await _tokenClient.PostAsync(tokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (tokenData?.AccessToken != null)
                    {
                        _cachedToken = tokenData.AccessToken;
                        int expirySeconds = tokenData.ExpiresIn > 0 ? tokenData.ExpiresIn : 900;
                        _tokenExpiration = DateTime.UtcNow.AddSeconds(expirySeconds - 30);

                        _logger.LogInformation("Successfully retrieved new Service Token. Expires at {ExpirationTime} (UTC).", _tokenExpiration);
                        return _cachedToken;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to retrieve Access Token. Status: {StatusCode}. IAM Response: {ErrorContent}",
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FATAL ERROR during token retrieval from IAM at {Endpoint}. Check connection, Kestrel, or certificate settings.", tokenEndpoint);
            }

            return null;
        }
    }
}