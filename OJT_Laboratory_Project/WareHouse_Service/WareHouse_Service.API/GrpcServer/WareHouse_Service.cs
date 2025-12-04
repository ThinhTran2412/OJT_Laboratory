using Grpc.Core;
using WareHouse_Service.API.Protos;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using WareHouse_Service.Application.Instruments.Commands;

namespace WareHouse_Service.API.GrpcServer
{
    public class WareHouse_Service : WarehouseService.WarehouseServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WareHouse_Service> _logger;

        public WareHouse_Service(IMediator mediator, ILogger<WareHouse_Service> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<ProcessTestOrderResponse> ProcessTestOrder(
            ProcessTestOrderRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation("Received ProcessTestOrder request for OrderId: {OrderId}", request.OrderId);

            // Parse OrderId từ string sang Guid
            if (!Guid.TryParse(request.OrderId, out var testOrderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid OrderId format. Must be a valid GUID."));
            }

            // Validate TestType
            if (string.IsNullOrWhiteSpace(request.TestType))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "TestType is required and cannot be empty."));
            }

            // Gọi CQRS Handler
            var result = await _mediator.Send(new ProcessTestOrderCommand(testOrderId, request.TestType));

            // Map kết quả sang gRPC Response
            var response = new ProcessTestOrderResponse
            {
                TestOrderId = result.TestOrderId.ToString(),
                Instrument = result.Instrument,
                PerformedDate = Timestamp.FromDateTime(result.PerformedDate.ToUniversalTime())
            };

            foreach (var r in result.Results)
            {
                response.Results.Add(new RawResultItem
                {
                    TestCode = r.TestCode,
                    Parameter = r.Parameter,
                    Unit = r.Unit,
                    ReferenceRange = r.ReferenceRange,
                    Status = r.Status,
                    ValueNumeric = r.ValueNumeric ?? 0,
                    ValueText = r.ValueText ?? string.Empty
                });
            }

            _logger.LogInformation("Processed TestOrderId: {TestOrderId} using Instrument: {Instrument}",
                result.TestOrderId, result.Instrument);

            return response;
        }
    }
}
