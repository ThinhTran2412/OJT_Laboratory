using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Infrastructure.Repositories
{
    /// <summary>
    /// Implement method form IPatientService
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IPatientService" />
    public class PatientService : IPatientService
    {
        /// <summary>
        /// Implement method from IPatientService
        /// </summary>
        /// <seealso cref="Laboratory_Service.Application.Interface.IPatientService" />
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;
        private readonly IIAMService _userService;
        private readonly IEncryptionService _encrypt;

        public PatientService(
            IPatientRepository patientRepository,
            ILogger<PatientService> logger,
            IIAMService userService,
            IEncryptionService encrypt)
        {
            _patientRepository = patientRepository;
            _logger = logger;
            _userService = userService;
            _encrypt = encrypt;
        }

        /// <summary>
        /// Synchronizes the patient with user asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <param name="updatedBy">The updated by.</param>
        /// <returns></returns>
        public async Task<Patient?> SynchronizePatientWithUserAsync(string identifyNumber, string updatedBy)
        {
            try
            {
                _logger.LogInformation("Synchronizing patient with user data for IdentifyNumber: {IdentifyNumber}", identifyNumber);

                var encryptedIdentify = _encrypt.Encrypt(identifyNumber);

                var patient = await _patientRepository.GetByIdentifyNumberAsync(encryptedIdentify);
                if (patient == null)
                {
                    _logger.LogWarning("Patient not found for IdentifyNumber: {IdentifyNumber}", identifyNumber);
                    return null;
                }

                var userData = await _userService.GetUserByIdentifyNumberAsync(identifyNumber);
                if (userData == null)
                {
                    _logger.LogWarning("User not found in IAM Service for IdentifyNumber: {IdentifyNumber}", identifyNumber);
                    return patient;
                }

                bool hasChanges = false;

                if (patient.FullName != userData.FullName) { patient.FullName = userData.FullName; hasChanges = true; }
                if (patient.Email != userData.Email) { patient.Email = userData.Email; hasChanges = true; }
                if (patient.PhoneNumber != userData.PhoneNumber) { patient.PhoneNumber = userData.PhoneNumber; hasChanges = true; }
                if (patient.Address != userData.Address) { patient.Address = userData.Address; hasChanges = true; }

                if (hasChanges)
                {
                    patient.UpdatedAt = DateTime.UtcNow;

                    var updatedPatient = await _patientRepository.UpdateAsync(patient);
                    _logger.LogInformation("Successfully synchronized patient with user data. PatientId: {PatientId}", updatedPatient.PatientId);
                    return updatedPatient;
                }
                else
                {
                    _logger.LogInformation("No changes needed for patient synchronization. PatientId: {PatientId}", patient.PatientId);
                    return patient;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing patient with user data for IdentifyNumber: {IdentifyNumber}", identifyNumber);
                return null;
            }
        }

        /// <summary>
        /// Creates a patient from IAM user data
        /// </summary>
        public async Task<Patient?> CreatePatientFromUserAsync(CreatePatientClient createPatientDto, string createdBy)
        {
            if (createPatientDto == null)
                return null;

            try
            {
                _logger.LogInformation("Creating patient for IdentifyNumber: {IdentifyNumber}", createPatientDto.IdentifyNumber);

                var encryptedIdentify = _encrypt.Encrypt(createPatientDto.IdentifyNumber);

                var existingPatient = await _patientRepository.GetByIdentifyNumberAsync(encryptedIdentify);
                if (existingPatient != null)
                {
                    _logger.LogWarning("Patient already exists for IdentifyNumber: {IdentifyNumber}", createPatientDto.IdentifyNumber);
                    return existingPatient;
                }

                var patient = new Patient
                {
                    IdentifyNumber = createPatientDto.IdentifyNumber,
                    FullName = createPatientDto.FullName,
                    Gender = createPatientDto.Gender,
                    DateOfBirth = createPatientDto.DateOfBirth,
                    PhoneNumber = createPatientDto.PhoneNumber ?? string.Empty,
                    Email = createPatientDto.Email ?? string.Empty,
                    Address = createPatientDto.Address ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                var createdPatient = await _patientRepository.AddAsync(patient);
                _logger.LogInformation("Successfully created patient. PatientId: {PatientId}", createdPatient.PatientId);

                return createdPatient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient for IdentifyNumber: {IdentifyNumber}", createPatientDto.IdentifyNumber);
                return null;
            }
        }

        public async Task<PatientDto?> GetPatientByIdentityNumberAsync(string identifyNumber)
        {
            var patient = await _patientRepository.GetByIdentifyNumberAsync(identifyNumber);

            if (patient == null)
                return null;

            return new PatientDto
            {
                PatientId = patient.PatientId,
                FullName = patient.FullName,
                Gender = patient.Gender,
                DateOfBirth = patient.DateOfBirth,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,
                IdentifyNumber = patient.IdentifyNumber,
                Email = patient.Email,
                Age = patient.Age
            };
        }
    }
}
