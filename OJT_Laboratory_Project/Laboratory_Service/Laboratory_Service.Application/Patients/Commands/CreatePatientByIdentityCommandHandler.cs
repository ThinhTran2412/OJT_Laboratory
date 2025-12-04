using AutoMapper;
using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.Patients.Commands
{
    /// <summary>
    /// Handle for CreatePatientByIdentityCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Patients.Commands.CreatePatientByIdentityCommand, Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;" />
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Patients.Commands.CreatePatientByIdentityCommand, Laboratory_Service.Application.DTOs.Patients.PatientDto&gt;" />
    public class CreatePatientByIdentityCommandHandler : IRequestHandler<CreatePatientByIdentityCommand, PatientDto>
    {
        // Khai báo fields
        /// <summary>
        /// The patient repository
        /// </summary>
        private readonly IPatientRepository _patientRepository;
        /// <summary>
        /// The iam service
        /// </summary>
        private readonly IIAMService _iamService;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// The encrypt
        /// </summary>
        private readonly IEncryptionService _encrypt;
        /// <summary>
        /// The iam user service
        /// </summary>
        private readonly IIamUserService _iamUserService;

        // Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePatientByIdentityCommandHandler"/> class.
        /// </summary>
        /// <param name="patientRepository">The patient repository.</param>
        /// <param name="iamService">The iam service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="encrypt">The encrypt.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="iamUserService">The iam user service.</param>
        public CreatePatientByIdentityCommandHandler(
            IPatientRepository patientRepository,
            IIAMService iamService,
            IMapper mapper,
            IEncryptionService encrypt,
            ILogger<CreatePatientByIdentityCommandHandler> logger,
            IIamUserService iamUserService)
        {
            _patientRepository = patientRepository;
            _iamService = iamService;
            _mapper = mapper;
            _encrypt = encrypt;
            _logger = logger;
            _iamUserService = iamUserService;
        }

        // --- Handle Method ---

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Patient with this identification number already exists
        /// or
        /// Could not create User in IAM Service for IdentifyNumber: {plainIdentifyNumber}
        /// </exception>
        public async Task<PatientDto> Handle(CreatePatientByIdentityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var plainIdentifyNumber = request.IdentifyNumber;

                var encryptedIdentify = _encrypt.Encrypt(plainIdentifyNumber);

                var existingPatient = await _patientRepository.GetByIdentifyNumberAsync(encryptedIdentify);
                if (existingPatient != null)
                    throw new InvalidOperationException("Patient with this identification number already exists");

                UserDataDTO? userInfo;

                userInfo = await _iamUserService.GetUserByIdentifyNumberAsync(plainIdentifyNumber);

                if (userInfo == null)
                {
                    _logger.LogInformation("User not found in IAM. Attempting to create new user using request data.");

                    // Validate IdentifyNumber before creating user in IAM
                    if (string.IsNullOrWhiteSpace(plainIdentifyNumber))
                    {
                        throw new InvalidOperationException("IdentifyNumber is required to create a user in IAM Service. Cannot create patient without IdentifyNumber.");
                    }

                    var newUserDetails = new UserDataDTO(
                        plainIdentifyNumber,
                        request.FullName,
                        request.DateOfBirth,
                        request.Gender,
                        request.PhoneNumber,
                        request.Email,
                        request.Address,
                        5,
                        DateTime.Now.Year - request.DateOfBirth.Year
                    );

                    var isCreated = await _iamUserService.CreateUserAsync(newUserDetails);

                    if (!isCreated)
                    {
                        throw new InvalidOperationException($"Could not create User in IAM Service for IdentifyNumber: {plainIdentifyNumber}");
                    }
                    userInfo = newUserDetails;
                    _logger.LogInformation("Successfully created User in IAM and preparing to create Patient.");
                }
                else
                {
                    _logger.LogInformation("User found in IAM. Using IAM data to create Patient.");
                }
                var patient = new Patient
                {
                    IdentifyNumber = plainIdentifyNumber,
                    FullName = userInfo.FullName,
                    DateOfBirth = userInfo.DateOfBirth,
                    Gender = userInfo.Gender,
                    PhoneNumber = userInfo.PhoneNumber,
                    Email = userInfo.Email,
                    Address = userInfo.Address,

                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedAt = null,
                };

                var createdPatient = await _patientRepository.AddAsync(patient);

                return _mapper.Map<PatientDto>(createdPatient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient for IdentifyNumber {IdentifyNumber}", request.IdentifyNumber);
                throw; 
            }
        }
    }
}