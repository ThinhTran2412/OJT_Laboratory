using Grpc.Core;
using Laboratory_Service.Application.DTOs.TestResult;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.Test_Result.Commands
{
    public class ProcessTestResultMessageCommandHandler 
        : IRequestHandler<ProcessTestResultMessageCommand, TestResultIngressResponseDto>
    {
        private readonly ITestOrderRepository _testOrderRepo;
        private readonly ITestResultRepository _testResultRepo;
        private readonly IFlaggingService _flaggingService;
        private readonly IEventLogService _eventLogService;
        private readonly IProcessedMessageRepository _processedMessageRepo;
        private readonly ILogger<ProcessTestResultMessageCommandHandler> _logger;
        private readonly IWareHouseGrpcClient _wareHouseGrpcClient;

        public ProcessTestResultMessageCommandHandler(
            ITestOrderRepository testOrderRepo,
            ITestResultRepository testResultRepo,
            IFlaggingService flaggingService,
            IEventLogService eventLogService,
            IProcessedMessageRepository processedMessageRepo,
            ILogger<ProcessTestResultMessageCommandHandler> logger,
            IWareHouseGrpcClient wareHouseGrpcClient)
        {
            _testOrderRepo = testOrderRepo;
            _testResultRepo = testResultRepo;
            _flaggingService = flaggingService;
            _eventLogService = eventLogService;
            _processedMessageRepo = processedMessageRepo;
            _logger = logger;
            _wareHouseGrpcClient = wareHouseGrpcClient;
        }

        public async Task<TestResultIngressResponseDto> Handle(ProcessTestResultMessageCommand request, CancellationToken cancellationToken)
        {
            var sourceSystem = "Simulator_gRPC";
            var testOrderId = request.TestOrderId;
            var messageId = $"Temporary_{testOrderId}";

            _logger.LogInformation("START processing via gRPC pull. TestOrderId: {TestOrderId}", testOrderId);

            try
            {
                // STEP 1: Call gRPC for raw results
                var rawResultDto = await _wareHouseGrpcClient.ProcessTestOrder(testOrderId, request.TestType);

                if (rawResultDto == null || !rawResultDto.Results.Any())
                {
                    var msg = rawResultDto == null ? "gRPC call returned null." : "No results returned.";
                    _logger.LogWarning("Failed to retrieve results for TestOrderId {TestOrderId}: {Message}", testOrderId, msg);

                    return new TestResultIngressResponseDto { Success = false, Message = $"Failed to retrieve results: {msg}" };
                }

                var instrument = rawResultDto.Instrument;
                var performedDate = rawResultDto.PerformedDate;
                var resultsDto = rawResultDto.Results;

                messageId = $"{testOrderId}_{performedDate:yyyyMMddHHmmss}";

                // STEP 3: Idempotency
                var isNewMessage = await _processedMessageRepo.TryAddIfNotExistsAsync(
                    messageId, sourceSystem, testOrderId, cancellationToken);

                if (!isNewMessage)
                {
                    var existingMessage = await _processedMessageRepo.GetByMessageIdAsync(messageId, cancellationToken);

                    return new TestResultIngressResponseDto
                    {
                        Success = true,
                        Message = $"Message {messageId} has already been processed (duplicate).",
                        MessageId = messageId,
                        CreatedCount = existingMessage?.CreatedCount ?? 0,
                        ProcessedAt = existingMessage?.ProcessedAt ?? DateTime.UtcNow
                    };
                }

                // STEP 4: Get TestOrder
                var testOrder = await _testOrderRepo.GetByIdAsync(testOrderId, cancellationToken);
                if (testOrder == null)
                {
                    var error = $"TestOrder with ID {testOrderId} not found.";
                    _logger.LogWarning(error);

                    return new TestResultIngressResponseDto { Success = false, Message = error, MessageId = messageId };
                }

                var gender = testOrder.MedicalRecord?.Patient?.Gender;

                // STEP 5: Mapping + Flagging
                var testResults = new List<TestResult>();
                var processedAt = DateTime.UtcNow;

                foreach (var resultDto in resultsDto)
                {
                    var flag = await _flaggingService.CalculateFlagAsync(
                        resultDto.TestCode,
                        resultDto.ValueNumeric,
                        gender,
                        cancellationToken);

                    // Logic chuẩn bạn muốn:
                    // Nếu status input khác flag → dùng flag làm ResultStatus
                    var resultStatus = flag != resultDto.Status
                        ? flag
                        : resultDto.Status;

                    var testResult = new TestResult
                    {
                        TestOrderId = testOrderId,
                        TestCode = resultDto.TestCode,
                        Parameter = resultDto.Parameter,
                        ValueNumeric = resultDto.ValueNumeric,
                        ValueText = resultDto.ValueText,
                        Unit = resultDto.Unit,
                        ReferenceRange = resultDto.ReferenceRange,
                        Instrument = instrument,

                        ResultStatus = resultStatus,
                        Flag = flag,
                        FlaggedAt = processedAt,
                        PerformedDate = performedDate,
                        CreatedAt = processedAt,

                        ReviewedByAI = false,
                        IsConfirmed = false
                    };

                    testResults.Add(testResult);
                }

                // STEP 6: Insert all TestResult
                var createdCount = await _testResultRepo.AddRangeAsync(testResults, cancellationToken);

                // Update processed message
                var processedMsg = await _processedMessageRepo.GetByMessageIdAsync(messageId, cancellationToken);
                if (processedMsg != null)
                {
                    processedMsg.CreatedCount = createdCount;
                    processedMsg.ProcessedAt = processedAt;
                    await _processedMessageRepo.UpdateAsync(processedMsg, cancellationToken);
                }

                // STEP 7: Event log
                await _eventLogService.CreateAsync(new EventLog
                {
                    EventId = "E_00010",
                    Action = "Process Test Result Message (gRPC)",
                    EventLogMessage = $"Processed test result message {messageId}. Created {createdCount} result(s).",
                    OperatorName = "System",
                    EntityType = "TestResult",
                    EntityId = testOrderId,
                    CreatedOn = processedAt
                });

                return new TestResultIngressResponseDto
                {
                    Success = true,
                    Message = $"Successfully processed message and created {createdCount} test results.",
                    MessageId = messageId,
                    CreatedCount = createdCount,
                    ProcessedAt = processedAt
                };
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC Error while calling Simulator. TestOrderId: {TestOrderId}", testOrderId);
                return new TestResultIngressResponseDto
                {
                    Success = false,
                    Message = $"gRPC Error: {ex.Status.Detail}",
                    ErrorDetails = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General Error processing test result message. TestOrderId: {TestOrderId}", testOrderId);
                return new TestResultIngressResponseDto
                {
                    Success = false,
                    Message = "An error occurred during processing.",
                    MessageId = messageId,
                    ErrorDetails = ex.Message
                };
            }
        }
    }
}
