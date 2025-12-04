using Grpc.Core;
using Microsoft.Extensions.Logging;
using Simulator.API.Protos.Query;
using WareHouse_Service.Application.DTOs;
using WareHouse_Service.Application.Interface;

namespace WareHouse_Service.Infrastructure.GrpcClient
{
    public class SimulatorGrpcClient : ISimulatorGrpcClient
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly RawDataQueryService.RawDataQueryServiceClient _client;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<SimulatorGrpcClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatorGrpcClient"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="logger">The logger.</param>
        public SimulatorGrpcClient(
            RawDataQueryService.RawDataQueryServiceClient client,
            ILogger<SimulatorGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Creates the and get raw results asynchronous.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        /// <param name="testType">The test type (not used in proto, kept for interface compatibility).</param>
        /// <returns></returns>
        public async Task<RawTestResultDTO?> CreateAndGetRawResultsAsync(Guid testOrderId, string testType)
        {
            var request = new CreateAndSendTestOrderRequest
            {
                TestOrderId = testOrderId.ToString()
            };

            try
            {
                _logger.LogInformation("Calling Simulator CreateAndSendTestOrderAsync for TestOrderId: {TestOrderId}", testOrderId);

                var response = await _client.CreateAndSendTestOrderAsync(request);


                if (string.IsNullOrEmpty(response.TestOrderId))
                {
                    _logger.LogWarning("Simulator created no data or returned an empty response for TestOrderId: {TestOrderId}", testOrderId);
                    return null;
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

                _logger.LogInformation("Successfully created and retrieved {Count} results via gRPC for TestOrderId: {TestOrderId}",
                    dto.Results.Count, testOrderId);

                return dto;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning("TestOrderId {TestOrderId} not found/created in Simulator", testOrderId);
                return null;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC error calling Simulator CreateAndSendTestOrder for TestOrderId: {TestOrderId}. Status: {StatusCode}",
                    testOrderId, ex.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Simulator CreateAndSendTestOrder for TestOrderId: {TestOrderId}", testOrderId);
                return null;
            }
        }
    }
}
