using WareHouse_Service.Application.Interface;
using WareHouse_Service.Domain.Entity;
using Microsoft.Extensions.Logging;
using WareHouse_Service.Application.DTOs;
using MediatR;

namespace WareHouse_Service.Application.Instruments.Commands
{
    public class ProcessTestOrderCommandHandler : IRequestHandler<ProcessTestOrderCommand, RawTestResultDTO>
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ISimulatorGrpcClient _simulatorGrpcClient;
        private readonly ILogger<ProcessTestOrderCommandHandler> _logger;
        public ProcessTestOrderCommandHandler(IInstrumentRepository instrumentRepository,ISimulatorGrpcClient simulatorGrpcClient,ILogger<ProcessTestOrderCommandHandler> logger)
        {
            _instrumentRepository = instrumentRepository;
            _simulatorGrpcClient = simulatorGrpcClient;
            _logger = logger;
        }
        public async Task<RawTestResultDTO> Handle(ProcessTestOrderCommand request ,CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.TestType))
                throw new ArgumentException("TestType is required");
            var instruments = await _instrumentRepository.GetByTestTypeAsync(request.TestType);
            if (!instruments.Any())
                throw new Exception($"No instruments support test type {request.TestType}");

            var selectedInstrument = instruments.First();

            var simResponse = await _simulatorGrpcClient.CreateAndGetRawResultsAsync(request.TestOrderId, selectedInstrument.InstrumentName);
            if(simResponse == null)
            {
                _logger.LogError("Failed to get raw test results from simulator for TestOrderId: {TestOrderId}", request.TestOrderId);
                throw new Exception("Failed to get raw test results from simulator");
            }
            _logger.LogInformation("Successfully processed TestOrderId: {TestOrderId} using Instrument: {InstrumentName}", request.TestOrderId, selectedInstrument.InstrumentName);
            return new RawTestResultDTO
            {
                TestOrderId = request.TestOrderId,
                Instrument = selectedInstrument.InstrumentName,
                PerformedDate = DateTime.UtcNow,
                Results = simResponse.Results
            };
        }
    }
}
