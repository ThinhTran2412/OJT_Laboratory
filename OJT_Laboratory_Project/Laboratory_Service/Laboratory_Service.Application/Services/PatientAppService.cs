using AutoMapper;

using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.Services
{
    /// <summary>
    /// Application service implementing patient use-cases. Works with repositories and maps to DTOs.
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IPatientAppService" />
    public class PatientAppService : IPatientAppService
    {
        /// <summary>
        /// The patient repository
        /// </summary>
        private readonly IPatientRepository _patientRepository;
        /// <summary>
        /// The medical record repository
        /// </summary>
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PatientAppService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientAppService"/> class.
        /// </summary>
        /// <param name="patientRepository">The patient repository.</param>
        /// <param name="medicalRecordRepository">The medical record repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public PatientAppService(
            IPatientRepository patientRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper,
            ILogger<PatientAppService> logger)
        {
            _patientRepository = patientRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets the by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        public async Task<PatientDto?> GetByIdentifyNumberAsync(string identifyNumber)
        {
            if (string.IsNullOrWhiteSpace(identifyNumber))
                return null;

            var patient = await _patientRepository.GetByIdentifyNumberAsync(identifyNumber);
            if (patient == null) return null;
            return _mapper.Map<PatientDto>(patient);
        }

        /// <summary>
        /// Existses the by identify number asynchronous.
        /// </summary>
        /// <param name="identifyNumber">The identify number.</param>
        /// <returns></returns>
        public async Task<bool> ExistsByIdentifyNumberAsync(string identifyNumber)
        {
            if (string.IsNullOrWhiteSpace(identifyNumber))
                return false;
            return await _patientRepository.ExistsByIdentifyNumberAsync(identifyNumber);
        }

        /// <summary>
        /// Creates the patient from user asynchronous.
        /// </summary>
        /// <param name="createPatientDto">The create patient dto.</param>
        /// <param name="createdBy">The created by.</param>
        /// <returns></returns>
        public async Task<PatientDto?> CreatePatientFromUserAsync(CreatePatientClient createPatientDto, string createdBy)
        {
            if (createPatientDto == null)
                return null;

            // business rule: if already exists, return existing DTO
            var exists = await _patientRepository.ExistsByIdentifyNumberAsync(createPatientDto.IdentifyNumber);
            if (exists)
            {
                var existing = await _patientRepository.GetByIdentifyNumberAsync(createPatientDto.IdentifyNumber);
                return existing == null ? null : _mapper.Map<PatientDto>(existing);
            }

            // Map DTO -> Domain entity and persist
            var patientEntity = _mapper.Map<Domain.Entity.Patient>(createPatientDto);
            // set fields not present in CreatePatientDto
            patientEntity.CreatedBy = createdBy ?? string.Empty;
            patientEntity.CreatedAt = DateTime.UtcNow;

            var created = await _patientRepository.AddAsync(patientEntity);
            return _mapper.Map<PatientDto>(created);
        }


        /// <summary>
        /// Gets the patients by identify numbers asynchronous.
        /// </summary>
        /// <param name="identifyNumbers">The identify numbers.</param>
        /// <returns></returns>
        public async Task<List<PatientDto>> GetPatientsByIdentifyNumbersAsync(IEnumerable<string> identifyNumbers)
        {
            var result = new List<PatientDto>();
            if (identifyNumbers == null) return result;

            foreach (var id in identifyNumbers)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                var patient = await _patientRepository.GetByIdentifyNumberAsync(id);
                if (patient != null)
                    result.Add(_mapper.Map<PatientDto>(patient));
            }

            return result;
        }
    }
}
