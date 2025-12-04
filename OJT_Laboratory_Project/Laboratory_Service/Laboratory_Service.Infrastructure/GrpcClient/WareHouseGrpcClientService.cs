using Laboratory_Service.Application.DTOs.TestResult;
using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Logging;
using WareHouse_Service.API.Protos;

namespace Laboratory_Service.Infrastructure.GrpcClient
{
    public class WareHouseGrpcClientService : IWareHouseGrpcClient
    {
        private readonly ILogger<WareHouseGrpcClientService> _logger;
        private readonly WarehouseService.WarehouseServiceClient _wareHouseServiceClient;

        public WareHouseGrpcClientService(
            WarehouseService.WarehouseServiceClient wareHouseServiceClient,
            ILogger<WareHouseGrpcClientService> logger)
        {
            _wareHouseServiceClient = wareHouseServiceClient;
            _logger = logger;
        }
        public async Task<RawTestResultDTO?> ProcessTestOrder(Guid testOrderId, string testType)
        {
            _logger.LogInformation("Processing test order with ID: {TestOrderId} and TestType: {TestType}", testOrderId, testType);
            var request = new ProcessTestOrderRequest
            {
                OrderId = testOrderId.ToString(),
                TestType = testType
            };
            var response = await _wareHouseServiceClient.ProcessTestOrderAsync(request);
            if (response == null)
            {
                _logger.LogWarning("No response received for test order ID: {TestOrderId}", testOrderId);
                return await Task.FromResult<RawTestResultDTO?>(null);
            }
            var dto = new RawTestResultDTO
            {
                TestOrderId = new Guid(response.TestOrderId),
                Instrument = response.Instrument,
                PerformedDate = response.PerformedDate.ToDateTime(),
                Results = response.Results.Select(item => new RawResultItemDTO
                {
                    TestCode = item.TestCode,
                    Parameter = item.Parameter,
                    ValueNumeric = item.ValueNumeric,
                    ValueText = item.ValueText,
                    Unit = item.Unit,
                    ReferenceRange = item.ReferenceRange,
                    Status = item.Status
                }).ToList()
            };
            _logger.LogInformation("Successfully processed test order ID: {TestOrderId}", testOrderId);
            return dto;

        }
    }
}
