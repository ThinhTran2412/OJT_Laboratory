using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Handle modifiel TestOrder command
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Commands.ModifyTestOrderCommand, System.Boolean&gt;" />
    public class ModifyTestOrderCommandHandler : IRequestHandler<ModifyTestOrderCommand, bool>
    {
        /// <summary>
        /// The order repo
        /// </summary>
        private readonly ITestOrderRepository _orderRepo;
        /// <summary>
        /// The record repo
        /// </summary>
        private readonly IMedicalRecordRepository _recordRepo;
        /// <summary>
        /// The patient service
        /// </summary>
        private readonly IPatientService _patientService;
        /// <summary>
        /// The event log repo
        /// </summary>
        private readonly IEventLogService _eventLogRepo;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ModifyTestOrderCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyTestOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="orderRepo">The order repo.</param>
        /// <param name="recordRepo">The record repo.</param>
        /// <param name="patientService">The patient service.</param>
        /// <param name="eventLogRepo">The event log repo.</param>
        /// <param name="logger">The logger.</param>
        public ModifyTestOrderCommandHandler(
            ITestOrderRepository orderRepo,
            IMedicalRecordRepository recordRepo,
            IPatientService patientService,
            IEventLogService eventLogRepo,
            ILogger<ModifyTestOrderCommandHandler> logger)
        {
            _orderRepo = orderRepo;
            _recordRepo = recordRepo;
            _patientService = patientService;
            _eventLogRepo = eventLogRepo;
            _logger = logger;
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
        /// TestOrder not found: {request.TestOrderId}
        /// or
        /// Cannot synchronize patient with IAM Service.
        /// </exception>
        public async Task<bool> Handle(ModifyTestOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start modifying TestOrder {OrderId}", request.TestOrderId);

            var order = await _orderRepo.GetByIdAsync(request.TestOrderId, cancellationToken)
                ?? throw new InvalidOperationException($"TestOrder not found: {request.TestOrderId}");

            var syncedPatient = await _patientService.SynchronizePatientWithUserAsync(
                request.IdentifyNumber, request.UpdatedBy.ToString()
            ) ?? throw new InvalidOperationException("Cannot synchronize patient with IAM Service.");

            MedicalRecord? currentMedicalRecord = await _recordRepo.GetMedicalRecordById(syncedPatient.PatientId);

            if (currentMedicalRecord == null)
            {
                _logger.LogInformation("No medical record found. Creating new record for PatientId: {PatientId}", syncedPatient.PatientId);

                currentMedicalRecord = new MedicalRecord
                {
                    PatientId = syncedPatient.PatientId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.UpdatedBy.ToString(),
                    IsDeleted = false
                };

                await _recordRepo.AddAsync(currentMedicalRecord);
                _logger.LogInformation("New Medical Record created with Id: {RecordId}", currentMedicalRecord.MedicalRecordId);
            }
            else
            {
                _logger.LogInformation("Using existing Medical Record Id: {RecordId}", currentMedicalRecord.MedicalRecordId);
            }

            if (order.MedicalRecordId != currentMedicalRecord.MedicalRecordId)
            {
                order.MedicalRecordId = currentMedicalRecord.MedicalRecordId;
                _logger.LogInformation("TestOrder re-linked to Medical Record Id: {RecordId}", currentMedicalRecord.MedicalRecordId);
            }
            order.Priority = request.Priority;
            order.Status = request.Status;
            order.Note = request.Note;
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = request.UpdatedBy;

            await _orderRepo.SaveChangesAsync(cancellationToken);

            string logMessage = $"Test Order {order.OrderCode} modified by {request.UpdatedBy}.";

            var updateByUsername = request.UpdatedBy;
            await _eventLogRepo.CreateAsync(new EventLog
            {
                EventId = "E_00002",
                Action = "Modify Test Order",
                EventLogMessage = logMessage,
                OperatorName = updateByUsername,
                EntityType = "TestOrder",
                EntityId = order.TestOrderId,
                CreatedOn = DateTime.UtcNow
            });

            _logger.LogInformation("Modified TestOrder {OrderId} successfully", order.TestOrderId);
            return true;
        }
    }
}