using Laboratory_Service.Application.DTOs.Patients;
using Laboratory_Service.Application.DTOs.User;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Handle Create Test Order
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Commands.CreateTestOrderCommand, System.Guid&gt;" />
    public class CreateTestOrderCommandHandler : IRequestHandler<CreateTestOrderCommand, Guid>
    {
        /// <summary>
        /// The iam client
        /// </summary>
        private readonly IIamUserService _iamClient;
        /// <summary>
        /// The patient repo
        /// </summary>
        private readonly IPatientRepository _patientRepo;
        /// <summary>
        /// The record repo
        /// </summary>
        private readonly IMedicalRecordRepository _recordRepo;
        /// <summary>
        /// The order repo
        /// </summary>
        private readonly ITestOrderRepository _orderRepo;
        /// <summary>
        /// The patient service
        /// </summary>
        private readonly IPatientService _patientService;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<CreateTestOrderCommandHandler> _logger;
        private readonly IEncryptionService _encrypt;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTestOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="iamClient">The iam client.</param>
        /// <param name="patientRepo">The patient repo.</param>
        /// <param name="recordRepo">The record repo.</param>
        /// <param name="orderRepo">The order repo.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="patientService">The patient service.</param>
        public CreateTestOrderCommandHandler(
            IIamUserService iamClient,
            IPatientRepository patientRepo,
            IMedicalRecordRepository recordRepo,
            ITestOrderRepository orderRepo,
            IEncryptionService encrypt,
            ILogger<CreateTestOrderCommandHandler> logger,
            IPatientService patientService)
        {
            _iamClient = iamClient;
            _patientRepo = patientRepo;
            _recordRepo = recordRepo;
            _orderRepo = orderRepo;
            _logger = logger;
            _encrypt = encrypt;
            _patientService = patientService;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// User with IdentifyNumber '{request.IdentifyNumber}' does not exist in IAM Service.
        /// or
        /// Unable to create patient from IAM Service data.
        /// </exception>
        public async Task<Guid> Handle(CreateTestOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Test Order creation for IdentifyNumber: {IdentifyNumber}", request.IdentifyNumber);

            var plainId = request.IdentifyNumber;
            var encryptedId = _encrypt.Encrypt(plainId);

            var patient = await _patientRepo.GetByIdentifyNumberAsync(encryptedId);

            if (patient == null)
            {
                var createPatientDto = new CreatePatientClient
                {
                    IdentifyNumber = request.IdentifyNumber,
                    FullName = request.FullName ?? string.Empty,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender ?? string.Empty,
                    PhoneNumber = request.PhoneNumber ?? string.Empty,
                    Email = request.Email ?? string.Empty,
                    Address = request.Address ?? string.Empty
                };

                patient = await _patientService.CreatePatientFromUserAsync(createPatientDto, request.CreatedBy.ToString());

                if (patient == null)
                    throw new InvalidOperationException("Unable to create patient in database.");
            }

            var accountExists = await _iamClient.CheckUserExistsAsync(plainId);

            if (!accountExists)
            {
                _logger.LogInformation("Patient DOB: {DOB}, Calculated Age: {Age}", patient.DateOfBirth, patient.Age);
                var userDto = new UserDataDTO(
                    plainId,
                    patient.FullName,
                    patient.DateOfBirth,
                    patient.Gender,
                    patient.PhoneNumber ?? string.Empty,
                    patient.Email ?? string.Empty,
                    patient.Address ?? string.Empty,
                    RoleId: 5,
                    patient.Age
                );

                var created = await _iamClient.CreateUserAsync(userDto);
                if (!created)
                    _logger.LogWarning("Failed to create IAM user for PatientId: {PatientId}", patient.PatientId);
                else
                    _logger.LogInformation("Successfully created IAM user for PatientId: {PatientId}", patient.PatientId);
            }
            else
            {
                var accountInfo = await _iamClient.GetUserByIdentifyNumberAsync(plainId);

                bool isUpdated = false;

                if (!string.IsNullOrEmpty(accountInfo.FullName) && patient.FullName != accountInfo.FullName)
                {
                    patient.FullName = accountInfo.FullName;
                    isUpdated = true;
                }

                if (!string.IsNullOrEmpty(accountInfo.PhoneNumber) && patient.PhoneNumber != accountInfo.PhoneNumber)
                {
                    patient.PhoneNumber = accountInfo.PhoneNumber;
                    isUpdated = true;
                }

                if (!string.IsNullOrEmpty(accountInfo.Email) && patient.Email != accountInfo.Email)
                {
                    patient.Email = accountInfo.Email;
                    isUpdated = true;
                }

                if (!string.IsNullOrEmpty(accountInfo.Gender) && patient.Gender != accountInfo.Gender)
                {
                    patient.Gender = accountInfo.Gender;
                    isUpdated = true;
                }

                if (!string.IsNullOrEmpty(accountInfo.Address) && patient.Address != accountInfo.Address)
                {
                    patient.Address = accountInfo.Address;
                    isUpdated = true;
                }

                if (accountInfo.DateOfBirth != default && patient.DateOfBirth != accountInfo.DateOfBirth)
                {
                    patient.DateOfBirth = accountInfo.DateOfBirth;
                    isUpdated = true;
                }

                if (isUpdated)
                    await _patientRepo.UpdateAsync(patient);
            }

            var medicalRecord = (await _recordRepo.GetByPatientIdAsync(patient.PatientId))?.FirstOrDefault();
            if (medicalRecord == null)
            {
                medicalRecord = new MedicalRecord
                {
                    PatientId = patient.PatientId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy.ToString(),
                    IsDeleted = false
                };
                await _recordRepo.AddAsync(medicalRecord);
            }

            var testOrder = new TestOrder
            {
                TestOrderId = Guid.NewGuid(),
                OrderCode = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 6)}",
                TestType = request.TestType,
                Priority = request.Priority,
                Note = request.Note,
                Status = "Created",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy,
                MedicalRecordId = medicalRecord.MedicalRecordId
            };

            await _orderRepo.AddAsync(testOrder, cancellationToken);

            return testOrder.TestOrderId;
        }
    }
}
