using Laboratory_Service.Application.DTOs.MedicalRecords;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Application.MedicalRecords.Commands;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handle for AddMedicalRecordCommandHandler
/// </summary>
public class AddMedicalRecordCommandHandler : IRequestHandler<AddMedicalRecordCommand, CreateMedicalRecordResultDto>
{
    /// <summary>
    /// The medical record repository
    /// </summary>
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    /// <summary>
    /// The patient repository
    /// </summary>
    private readonly IPatientRepository _patientRepository;
    /// <summary>
    /// The logger
    /// </summary>
    private readonly ILogger<AddMedicalRecordCommandHandler> _logger;
    private readonly IEncryptionService _encryptionService;
    private readonly IIamUserService _iamClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddMedicalRecordCommandHandler"/> class.
    /// </summary>
    /// <param name="medicalRecordRepository">The medical record repository.</param>
    /// <param name="patientRepository">The patient repository.</param>
    /// <param name="logger">The logger.</param>
    public AddMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IPatientRepository patientRepository,
        ILogger<AddMedicalRecordCommandHandler> logger,
        IEncryptionService encryptionService,
        IIamUserService iamClient)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _logger = logger;
        _encryptionService = encryptionService;
        _iamClient = iamClient;
    }

    /// <summary>
    /// Handles a request
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Response from the request
    /// </returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Patient not found</exception>
    public async Task<CreateMedicalRecordResultDto> Handle(AddMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        // 1. Mã hóa IdentifyNumber
        var encryptedIdentifyNumber = _encryptionService.Encrypt(request.IdentifyNumber);

        // 2. Kiểm tra patient tồn tại
        var patient = await _patientRepository.GetByIdentifyNumberAsync(encryptedIdentifyNumber);

        if (patient == null)
        {
            // 3. Tạo patient mới
            patient = new Patient
            {
                IdentifyNumber = request.IdentifyNumber,
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth,
                Email = request.Email ?? string.Empty,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                Address = request.Address ?? string.Empty
            };

            await _patientRepository.AddAsync(patient);
            _logger.LogInformation("Patient created for IdentifyNumber {IdentifyNumber}", request.IdentifyNumber);
        }
        else
        {
            _logger.LogInformation("Patient already exists. Skip creating new patient.");
        }

        // 4. Tạo MedicalRecord
        var medicalRecord = new MedicalRecord
        {
            PatientId = patient.PatientId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _medicalRecordRepository.AddAsync(medicalRecord);

        _logger.LogInformation("Medical record created for PatientId {PatientId} by {CreatedBy}",
            patient.PatientId, request.CreatedBy);

        // 5. Tạo IAM user nếu cần
        if (request.CreateIAMUser)
        {
            var userDto = new UserDataDTO(
                request.IdentifyNumber,
                request.FullName,
                request.DateOfBirth,
                request.Gender,
                request.PhoneNumber ?? string.Empty,
                request.Email ?? string.Empty,
                request.Address ?? string.Empty,
                RoleId: 5,
                patient.Age
            );
            var created = await _iamClient.CreateUserAsync(userDto);
        }

        return new CreateMedicalRecordResultDto
        {
            Success = true,
            Message = "Medical record created successfully."
        };
    }
}
