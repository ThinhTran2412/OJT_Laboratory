using Grpc.Core;
using IAM_Service.API.Protos;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Infrastructure.GrpcClients
{
    /// <summary>
    /// gRPC Client to communicate with IAM Service (User-related operations).
    /// </summary>
    public class IamUserGrpcClientService : IIamUserService
    {
        private readonly UserService.UserServiceClient _userServiceClient;
        private readonly ILogger<IamUserGrpcClientService> _logger;

        public IamUserGrpcClientService(
            UserService.UserServiceClient userServiceClient,
            ILogger<IamUserGrpcClientService> logger)
        {
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        #region Mapper
        private UserDataDTO ToUserDataModel(UserData userData)
        {
            const string dateFormat = "yyyy-MM-dd";
            if (!DateOnly.TryParseExact(userData.DateOfBirth, dateFormat, out DateOnly dob))
            {
                throw new FormatException($"Invalid date format from IAM Service: {userData.DateOfBirth}.");
            }

            return new UserDataDTO(
                userData.IdentifyNumber,
                userData.FullName,
                dob,
                userData.Gender,
                userData.PhoneNumber,
                userData.Email,
                userData.Address,
                userData.RoleId,
                userData.Age
            );
        }
        #endregion

        /// <summary>
        /// Gets user information from IAM Service by IdentifyNumber (Citizen ID).
        /// </summary>
        public async Task<UserDataDTO?> GetUserByIdentifyNumberAsync(string identifyNumber)
        {
            try
            {
                _logger.LogInformation("[gRPC Call] Getting user from IAM Service by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                _logger.LogInformation("[gRPC Debug] gRPC Client Type: {ClientType}", _userServiceClient.GetType().FullName);

                var request = new GetUserByIdentifyNumberRequest { IdentifyNumber = identifyNumber };
                // Increase timeout to 60 seconds for Render HTTPS public URL calls
                var callOptions = new Grpc.Core.CallOptions(deadline: DateTime.UtcNow.AddSeconds(60)); // 60 second timeout
                _logger.LogInformation("[gRPC Debug] Starting gRPC call with deadline: {Deadline}", callOptions.Deadline);
                _logger.LogInformation("[gRPC Debug] Request IdentifyNumber: {IdentifyNumber}", identifyNumber);
                
                var response = await _userServiceClient.GetUserByIdentifyNumberAsync(request, callOptions);
                
                _logger.LogInformation("[gRPC Debug] gRPC call completed successfully");

                if (response.Success && response.User != null)
                {
                    _logger.LogInformation("Successfully retrieved user from IAM Service");
                    return ToUserDataModel(response.User);
                }

                _logger.LogWarning("Failed to get user from IAM Service: {Message}", response.Message ?? "No message");
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "[gRPC Error] gRPC error getting user by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                _logger.LogError("[gRPC Debug] RpcException StatusCode: {StatusCode}", ex.StatusCode);
                _logger.LogError("[gRPC Debug] RpcException Status: {Status}", ex.Status.StatusCode);
                _logger.LogError("[gRPC Debug] RpcException Detail: {Detail}", ex.Status.Detail);
                _logger.LogError("[gRPC Debug] RpcException Message: {Message}", ex.Message);
                
                // Log inner exception details if available
                if (ex.Status.DebugException != null)
                {
                    _logger.LogError("[gRPC Debug] DebugException Type: {ExceptionType}", ex.Status.DebugException.GetType().FullName);
                    _logger.LogError("[gRPC Debug] DebugException Message: {Message}", ex.Status.DebugException.Message);
                    _logger.LogError("[gRPC Debug] DebugException StackTrace: {StackTrace}", ex.Status.DebugException.StackTrace);
                    
                    if (ex.Status.DebugException.InnerException != null)
                    {
                        _logger.LogError("[gRPC Debug] InnerException Type: {ExceptionType}", ex.Status.DebugException.InnerException.GetType().FullName);
                        _logger.LogError("[gRPC Debug] InnerException Message: {Message}", ex.Status.DebugException.InnerException.Message);
                        _logger.LogError("[gRPC Debug] InnerException StackTrace: {StackTrace}", ex.Status.DebugException.InnerException.StackTrace);
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[gRPC Error] Unexpected error getting user by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                _logger.LogError("[gRPC Debug] Exception Type: {ExceptionType}", ex.GetType().FullName);
                _logger.LogError("[gRPC Debug] Exception Message: {Message}", ex.Message);
                _logger.LogError("[gRPC Debug] Exception StackTrace: {StackTrace}", ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Checks if user exists in IAM Service by IdentifyNumber (Citizen ID).
        /// </summary>
        public async Task<bool> CheckUserExistsAsync(string identifyNumber)
        {
            try
            {
                _logger.LogInformation("Checking user existence in IAM Service by IdentifyNumber: {IdentifyNumber}", identifyNumber);

                var request = new CheckUserExistsRequest { IdentifyNumber = identifyNumber };
                var callOptions = new Grpc.Core.CallOptions(deadline: DateTime.UtcNow.AddSeconds(30)); // 30 second timeout
                var response = await _userServiceClient.CheckUserExistsAsync(request, callOptions);

                if (response.Success)
                {
                    _logger.LogInformation("User existence check completed: {Exists}", response.Exists);
                    return response.Exists;
                }

                _logger.LogWarning("Failed to check user existence in IAM Service: {Message}", response.Message ?? "No message");
                return false;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error checking user existence by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence by IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return false;
            }
        }

        /// <summary>
        /// Gets multiple users from IAM Service by list of IdentifyNumbers.
        /// </summary>
        public async Task<List<UserDataDTO>> GetUsersByIdentifyNumbersAsync(List<string> identifyNumbers)
        {
            try
            {
                _logger.LogInformation("Getting multiple users from IAM Service. Count: {Count}", identifyNumbers.Count);

                var request = new GetUsersByIdentifyNumbersRequest();
                request.IdentifyNumbers.AddRange(identifyNumbers);

                var callOptions = new Grpc.Core.CallOptions(deadline: DateTime.UtcNow.AddSeconds(30)); // 30 second timeout
                var response = await _userServiceClient.GetUsersByIdentifyNumbersAsync(request, callOptions);

                if (response.Success)
                {
                    _logger.LogInformation("Successfully retrieved {Count} users from IAM Service", response.Users.Count);
                    return response.Users.Select(u => ToUserDataModel(u)).ToList();
                }

                _logger.LogWarning("Failed to get users from IAM Service: {Message}", response.Message ?? "No message");
                return new List<UserDataDTO>();
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error getting multiple users from IAM Service");
                return new List<UserDataDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting multiple users from IAM Service");
                return new List<UserDataDTO>();
            }
        }

        /// <summary>
        /// Creates a new user in IAM Service using gRPC.
        /// </summary>
        public async Task<bool> CreateUserAsync(UserDataDTO userDetails)
        {
            try
            {
                _logger.LogInformation("[gRPC Call] Attempting to create user in IAM Service for IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);

                // 1️⃣ Prepare gRPC Request
                var request = new CreateUserRequest
                {
                    IdentifyNumber = userDetails.IdentifyNumber,
                    FullName = userDetails.FullName,
                    DateOfBirth = userDetails.DateOfBirth.ToString("yyyy-MM-dd"),
                    Gender = userDetails.Gender,
                    PhoneNumber = userDetails.PhoneNumber,
                    Email = userDetails.Email,
                    Address = userDetails.Address,
                    RoleId = userDetails.RoleId ?? 5,
                    Age = userDetails.Age
                };

                // 2️⃣ Call gRPC Service
                // Increase timeout to 60 seconds for Render HTTPS public URL calls
                var callOptions = new Grpc.Core.CallOptions(deadline: DateTime.UtcNow.AddSeconds(60)); // 60 second timeout
                _logger.LogInformation("[gRPC Debug] Starting gRPC CreateUser call with deadline: {Deadline}", callOptions.Deadline);
                _logger.LogInformation("[gRPC Debug] Request IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);
                var response = await _userServiceClient.CreateUserAsync(request, callOptions);
                _logger.LogInformation("[gRPC Debug] gRPC CreateUser call completed successfully");

                // 3️⃣ Handle response
                if (response.Success)
                {
                    _logger.LogInformation("Successfully created user in IAM Service: {IdentifyNumber}", userDetails.IdentifyNumber);
                    return true;
                }

                _logger.LogWarning("Failed to create user in IAM Service: {Message}", response.Message ?? "No message");
                return false;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.AlreadyExists)
            {
                _logger.LogWarning(ex, "User with IdentifyNumber {IdentifyNumber} already exists in IAM Service.", userDetails.IdentifyNumber);
                return false;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error creating user for IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating user for IdentifyNumber: {IdentifyNumber}", userDetails.IdentifyNumber);
                return false;
            }
        }
    }
}
