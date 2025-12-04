using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Laboratory_Service.Application.Services
{
    /// <summary>
    /// Implementation of IAM Service communication
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IIAMService" />
    public class IAMService : IIAMService
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<IAMService> _logger;
        /// <summary>
        /// The token service
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IAMService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="tokenService">The token service.</param>
        public IAMService(HttpClient httpClient, IConfiguration configuration, ILogger<IAMService> logger, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Get user information by identification number
        /// </summary>
        /// <param name="identifyNumber">User's identification number</param>
        /// <returns>
        /// User information or null if not found
        /// </returns>
        public async Task<UserInfo?> GetUserByIdentifyNumberAsync(string identifyNumber)
        {
            try
            {
                var token = await _tokenService.GetAccessTokenAsync();
                if (token == null)
                {
                    _logger.LogError("Authorization token could not be retrieved. Aborting request to IAM.");
                    return null;
                }

                var iamServiceUrl = _configuration["IAMService:BaseUrl"];
                var endpoint = $"{iamServiceUrl}/api/User/detailByIdentify?identifyNumber={identifyNumber}";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(endpoint);

                _logger.LogInformation("Call to IAM: {Url}, Status: {StatusCode}", endpoint, response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var userData = JsonSerializer.Deserialize<UserInfo>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return userData;
                }

                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("IAM returned error {Status}: {Body}", response.StatusCode, errorBody);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by identify number {IdentifyNumber}", identifyNumber);
                return null;
            }
        }


        /// <summary>
        /// Get user information by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>
        /// User information or null if not found
        /// </returns>
        public async Task<UserInfo?> GetUserByIdAsync(int userId)
        {
            try
            {
                var iamServiceUrl = _configuration["IAMService:BaseUrl"];
                var endpoint = $"{iamServiceUrl}/api/User/{userId}";

                _httpClient.DefaultRequestHeaders.Clear();
                // Add authorization header if needed

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var userData = JsonSerializer.Deserialize<UserInfo>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return userData;
                }

                _logger.LogWarning("Failed to get user by ID {UserId}. Status: {StatusCode}",
                    userId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Validate user permissions for patient operations
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="operation">Operation type (CREATE, READ, UPDATE, DELETE)</param>
        /// <returns>
        /// True if user has permission, false otherwise
        /// </returns>
        public async Task<bool> ValidateUserPermissionAsync(int userId, string operation)
        {
            try
            {
                var userRole = await GetUserRoleAsync(userId);
                if (string.IsNullOrEmpty(userRole))
                    return false;

                // Define role-based permissions
                var permissions = GetRolePermissions(userRole);
                return permissions.Contains(operation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user permission for user {UserId} and operation {Operation}",
                    userId, operation);
                return false;
            }
        }

        /// <summary>
        /// Get user role information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>
        /// User role information
        /// </returns>
        public async Task<string?> GetUserRoleAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                return user?.RoleName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user role for user {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Get role-based permissions
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        private static List<string> GetRolePermissions(string role)
        {
            return role.ToLower() switch
            {
                "admin" or "administrator" => new List<string> { "CREATE", "READ", "UPDATE", "DELETE" },
                "doctor" => new List<string> { "CREATE", "READ", "UPDATE" },
                "lab_supervisor" or "labsupervisor" => new List<string> { "CREATE", "READ", "UPDATE" },
                "lab_technician" or "labtechnician" => new List<string> { "CREATE", "READ", "UPDATE" },
                _ => new List<string> { "READ" }
            };
        }
        /// <summary>
        /// Gets the user by identify asynchronous.
        /// </summary>
        /// <param name="hashedIdentifyNumber">The hashed identify number.</param>
        /// <returns></returns>
        public async Task<UserDataDTO?> GetUserByIdentifyAsync(string hashedIdentifyNumber)
        {
            var iamServiceUrl = _configuration["IAMService:BaseUrl"];
            var endpoint = $"{iamServiceUrl}/api/User/detailByIdentify?identifyNumber={hashedIdentifyNumber}";

            var token = await _tokenService.GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(endpoint);

            _logger.LogInformation("IAMService call: {Endpoint}, status: {StatusCode}", endpoint, response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError("IAMService failed. Status {StatusCode}, Response: {Response}",
                    response.StatusCode, content);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserDataDTO>();
        }



    }
}
