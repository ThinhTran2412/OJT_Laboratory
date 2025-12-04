using Grpc.Core;
using Laboratory_Service.API.Protos;
using Laboratory_Service.API.Mappers;
using Laboratory_Service.Application.Interface;
using AutoMapper;
using Laboratory_Service.Application.DTOs.Patients;
using Microsoft.AspNetCore.Authorization;

namespace Laboratory_Service.API.Services
{
    /// <summary>
    /// gRPC service implementation for Patient Service
    /// Provides methods for other services to interact with Patient data via IdentifyNumber (Citizen ID)
    /// </summary>
    [AllowAnonymous]
    public class PatientGrpcService : PatientService.PatientServiceBase
    {
    private readonly IPatientAppService _patientAppService;
        private readonly ILogger<PatientGrpcService> _logger;
        private readonly IMapper _mapper;

        public PatientGrpcService(
            IPatientAppService patientAppService,
            ILogger<PatientGrpcService> logger,
            IMapper mapper)
        {
            _patientAppService = patientAppService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets patient information by IdentifyNumber (Citizen ID)
        /// </summary>
        /// <param name="request">Request containing IdentifyNumber</param>
        /// <param name="context">gRPC call context</param>
        /// <returns>Patient information or error response</returns>
        public override async Task<GetPatientByIdentifyNumberResponse> GetPatientByIdentifyNumber(
            GetPatientByIdentifyNumberRequest request, 
            ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting patient by IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);

                if (string.IsNullOrWhiteSpace(request.IdentifyNumber))
                {
                    return new GetPatientByIdentifyNumberResponse
                    {
                        Success = false,
                        Message = "IdentifyNumber cannot be null or empty"
                    };
                }

                var patientDto = await _patientAppService.GetByIdentifyNumberAsync(request.IdentifyNumber);
                
                if (patientDto == null)
                {
                    return new GetPatientByIdentifyNumberResponse
                    {
                        Success = false,
                        Message = $"Patient with IdentifyNumber '{request.IdentifyNumber}' not found"
                    };
                }

                return new GetPatientByIdentifyNumberResponse
                {
                    Success = true,
                    Message = "Patient found successfully",
                    Patient = patientDto.ToGrpcModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);
                return new GetPatientByIdentifyNumberResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Checks if patient exists by IdentifyNumber (Citizen ID)
        /// </summary>
        /// <param name="request">Request containing IdentifyNumber</param>
        /// <param name="context">gRPC call context</param>
        /// <returns>Existence check result</returns>
        public override async Task<CheckPatientExistsResponse> CheckPatientExists(
            CheckPatientExistsRequest request, 
            ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Checking patient existence by IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);

                if (string.IsNullOrWhiteSpace(request.IdentifyNumber))
                {
                    return new CheckPatientExistsResponse
                    {
                        Success = false,
                        Message = "IdentifyNumber cannot be null or empty",
                        Exists = false
                    };
                }

                var exists = await _patientAppService.ExistsByIdentifyNumberAsync(request.IdentifyNumber);

                return new CheckPatientExistsResponse
                {
                    Success = true,
                    Message = exists ? "Patient exists" : "Patient does not exist",
                    Exists = exists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking patient existence by IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);
                return new CheckPatientExistsResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Exists = false
                };
            }
        }

        /// <summary>
        /// Creates a new Patient from User data
        /// </summary>
        /// <param name="request">Request containing User data</param>
        /// <param name="context">gRPC call context</param>
        /// <returns>Created Patient or error response</returns>
        public override async Task<CreatePatientFromUserResponse> CreatePatientFromUser(
            CreatePatientFromUserRequest request, 
            ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Creating patient from user data for IdentifyNumber: {IdentifyNumber}", request.User.IdentifyNumber);

                if (request.User == null)
                {
                    return new CreatePatientFromUserResponse
                    {
                        Success = false,
                        Message = "User data cannot be null"
                    };
                }

                // Prepare CreatePatientDto and delegate to Application Service (use-case)
                var createDto = new CreatePatientClient
                {
                    IdentifyNumber = request.User.IdentifyNumber,
                    FullName = request.User.FullName,
                    DateOfBirth = DateOnly.Parse(request.User.DateOfBirth),
                    Gender = request.User.Gender,
                    PhoneNumber = request.User.PhoneNumber,
                    Email = request.User.Email,
                    Address = request.User.Address
                };

                var createdPatientDto = await _patientAppService.CreatePatientFromUserAsync(createDto, request.User.CreatedBy);

                if (createdPatientDto == null)
                {
                    return new CreatePatientFromUserResponse
                    {
                        Success = false,
                        Message = $"Patient with IdentifyNumber '{request.User.IdentifyNumber}' already exists or could not be created"
                    };
                }

                return new CreatePatientFromUserResponse
                {
                    Success = true,
                    Message = "Patient created successfully",
                    Patient = createdPatientDto.ToGrpcModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient from user data");
                return new CreatePatientFromUserResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Gets medical records for a patient by IdentifyNumber (Citizen ID)
        /// </summary>
        /// <param name="request">Request containing IdentifyNumber and pagination info</param>
        /// <param name="context">gRPC call context</param>
        /// <returns>Medical records or error response</returns>
        //public override async Task<GetMedicalRecordsByIdentifyNumberResponse> GetMedicalRecordsByIdentifyNumber(
        //    GetMedicalRecordsByIdentifyNumberRequest request,
        //    ServerCallContext context)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Getting medical records by IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);

        //        if (string.IsNullOrWhiteSpace(request.IdentifyNumber))
        //        {
        //            return new GetMedicalRecordsByIdentifyNumberResponse
        //            {
        //                Success = false,
        //                Message = "IdentifyNumber cannot be null or empty"
        //            };
        //        }

        //        // Delegate to application service to retrieve paginated medical records (returns DTO-based PaginatedMedicalRecordDto)
        //        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        //        var pageSize = request.PageSize > 0 ? request.PageSize : 10;

        //        var paginated = await _patientAppService.GetMedicalRecordsByIdentifyNumberAsync(request.IdentifyNumber, pageNumber, pageSize);

        //        if (paginated == null)
        //        {
        //            return new GetMedicalRecordsByIdentifyNumberResponse
        //            {
        //                Success = false,
        //                Message = $"Patient with IdentifyNumber '{request.IdentifyNumber}' not found or no records"
        //            };
        //        }

        //        var medicalRecordDataList = paginated.MedicalRecords.Select(r => r.ToGrpcModel()).ToList();

        //        return new GetMedicalRecordsByIdentifyNumberResponse
        //        {
        //            Success = true,
        //            Message = $"Found {paginated.TotalCount} medical records",
        //            MedicalRecords = { medicalRecordDataList },
        //            TotalCount = paginated.TotalCount,
        //            PageNumber = paginated.PageNumber,
        //            PageSize = paginated.PageSize
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting medical records by IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);
        //        return new GetMedicalRecordsByIdentifyNumberResponse
        //        {
        //            Success = false,
        //            Message = $"Internal server error: {ex.Message}"
        //        };
        //    }
        //}

        /// <summary>
        /// Gets multiple patients by list of IdentifyNumbers (Citizen IDs)
        /// </summary>
        /// <param name="request">Request containing list of IdentifyNumbers</param>
        /// <param name="context">gRPC call context</param>
        /// <returns>List of patients</returns>
        public override async Task<GetPatientsByIdentifyNumbersResponse> GetPatientsByIdentifyNumbers(
            GetPatientsByIdentifyNumbersRequest request, 
            ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting patients by IdentifyNumbers count: {Count}", request.IdentifyNumbers.Count);

                if (request.IdentifyNumbers == null || !request.IdentifyNumbers.Any())
                {
                    return new GetPatientsByIdentifyNumbersResponse
                    {
                        Success = false,
                        Message = "IdentifyNumbers list cannot be null or empty"
                    };
                }

                var patients = await _patientAppService.GetPatientsByIdentifyNumbersAsync(request.IdentifyNumbers);
                var patientDataList = patients.Select(p => p.ToGrpcModel()).ToList();
                
                return new GetPatientsByIdentifyNumbersResponse
                {
                    Success = true,
                    Message = $"Found {patients.Count} patients",
                    Patients = { patientDataList }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patients by IdentifyNumbers");
                return new GetPatientsByIdentifyNumbersResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }

    }
    }
