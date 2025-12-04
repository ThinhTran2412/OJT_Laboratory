using AutoMapper;
using Grpc.Core;
using IAM_Service.API.Protos;
using IAM_Service.Application.DTOs.User;
using IAM_Service.Application.Users.Command;
using IAM_Service.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace IAM_Service.API.Services
{
    /// <summary>
    /// gRPC service implementation for user-related operations, acting as a thin Presentation layer.
    /// </summary>
    [AllowAnonymous]
    public class UserGrpcService : UserService.UserServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserGrpcService> _logger;
        private readonly IMapper _mapper;

        public UserGrpcService(IMediator mediator, ILogger<UserGrpcService> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        private UserData MapToUserData(UserDetailDto userDto)
        {
            if (userDto == null)
            {
                return new UserData();
            }

            return new UserData
            {
                UserId = userDto.UserId,
                FullName = userDto.FullName ?? string.Empty,
                Email = userDto.Email ?? string.Empty,
                PhoneNumber = userDto.PhoneNumber ?? string.Empty,
                IdentifyNumber = userDto.IdentifyNumber ?? string.Empty,
                Gender = userDto.Gender ?? string.Empty,
                Age = userDto.Age,
                Address = userDto.Address ?? string.Empty,
                DateOfBirth = userDto.DateOfBirth.ToString("yyyy-MM-dd") ?? string.Empty,

                RoleId = userDto.RoleId ?? 0,
                RoleName = userDto.RoleName ?? string.Empty,
            };
        }
        // -------------------------------------------------------------------


        /// <summary>
        /// Get user information by IdentifyNumber (Citizen ID)
        /// </summary>
        public override async Task<GetUserByIdentifyNumberResponse> GetUserByIdentifyNumber(
            GetUserByIdentifyNumberRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("[gRPC Server] Received request for user with IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);
                _logger.LogInformation("[gRPC Server Debug] Request context - Host: {Host}, Method: {Method}", 
                    context.Host, context.Method);
                
                // Log request headers for debugging
                var schemeHeader = context.RequestHeaders.Get(":scheme");
                var userAgentHeader = context.RequestHeaders.Get("user-agent");
                var contentTypeHeader = context.RequestHeaders.Get("content-type");
                _logger.LogInformation("[gRPC Server Debug] Request headers - Scheme: {Scheme}, User-Agent: {UserAgent}, Content-Type: {ContentType}",
                    schemeHeader?.Value ?? "unknown", 
                    userAgentHeader?.Value ?? "unknown",
                    contentTypeHeader?.Value ?? "unknown");

                // Send Query via Mediator.
                var query = new GetUserDetailByIdentifyQuery(request.IdentifyNumber);
                var userDto = await _mediator.Send(query, context.CancellationToken);

                if (userDto == null)
                {
                    _logger.LogWarning("User not found for IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);
                    return new GetUserByIdentifyNumberResponse
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Convert Application DTO to Proto Model using Manual Mapping.
                var userDataProto = MapToUserData(userDto);

                return new GetUserByIdentifyNumberResponse
                {
                    Success = true,
                    Message = "User found",
                    User = userDataProto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by identify number: {IdentifyNumber}", request.IdentifyNumber);
                // Throw an RpcException for gRPC status if needed, but for simplicity, returning an error response:
                return new GetUserByIdentifyNumberResponse
                {
                    Success = false,
                    Message = "Error retrieving user information"
                };
            }
        }

        /// <summary>
        /// Check if a user exists by IdentifyNumber
        /// </summary>
        public override async Task<CheckUserExistsResponse> CheckUserExists(
            CheckUserExistsRequest request, ServerCallContext context)
        {
            try
            {
                // Send Query via Mediator.
                var query = new CheckUserExistsQuery(request.IdentifyNumber);
                var exists = await _mediator.Send(query, context.CancellationToken);

                return new CheckUserExistsResponse
                {
                    Success = true,
                    Message = exists ? "User exists" : "User does not exist", // Add message for clarity
                    Exists = exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user existence: {IdentifyNumber}", request.IdentifyNumber);
                return new CheckUserExistsResponse
                {
                    Success = false,
                    Message = "Error checking user existence",
                    Exists = false
                };
            }
        }

        /// <summary>
        /// Get users by multiple IdentifyNumbers
        /// </summary>
        public override async Task<GetUsersByIdentifyNumbersResponse> GetUsersByIdentifyNumbers(
            GetUsersByIdentifyNumbersRequest request, ServerCallContext context)
        {
            try
            {
                // Send Query to retrieve list of Users 
                var query = new GetAllUsersDetailByIdentifyNumbersQuery(request.IdentifyNumbers.ToList());
                // Assuming userDtos is List<UserDetailDto>
                var userDtos = await _mediator.Send(query, context.CancellationToken) ?? new List<UserDetailDto>();

                var response = new GetUsersByIdentifyNumbersResponse
                {
                    Success = true,
                    Message = $"Found {userDtos.Count} user(s)."
                };

                // Iterate through the DTO list and manually convert each element.
                foreach (var userDto in userDtos)
                {
                    response.Users.Add(MapToUserData(userDto));
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by identify numbers");
                return new GetUsersByIdentifyNumbersResponse
                {
                    Success = false,
                    Message = "Error retrieving user information"
                };
            }
        }

        public override async Task<CreateUserResponse> CreateUser(
    CreateUserRequest request,
    ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("[gRPC Server] Received CreateUser request for IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);
                _logger.LogInformation("[gRPC Server Debug] Request context - Host: {Host}, Method: {Method}", 
                    context.Host, context.Method);
                
                // Log request headers for debugging
                var schemeHeader = context.RequestHeaders.Get(":scheme");
                _logger.LogInformation("[gRPC Server Debug] Request header - Scheme: {Scheme}", schemeHeader?.Value ?? "unknown");
                // 1. Map gRPC request -> Command
                var command = new CreateUserCommand
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    IdentifyNumber = request.IdentifyNumber,
                    DateOfBirth = DateOnly.Parse(request.DateOfBirth),
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    RoleId = request.RoleId,
                    Age = request.Age
                };

                // 2. Send command to Application Layer
                await _mediator.Send(command, context.CancellationToken);

                // 3. Return success response
                return new CreateUserResponse
                {
                    Success = true,
                    Message = "User created successfully"
                };
            }
            catch (InvalidOperationException ex)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "Internal error occurred"
                };
            }
        }

    }
}