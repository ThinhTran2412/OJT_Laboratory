using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Simulator.API.Protos.Query;
using Simulator.Application.SimulateRawData.Command; 
using Simulator.Application.SimulateRawData.Query;
using static Simulator.API.Protos.Query.RawDataQueryService;

namespace Simulator.API.Service
{

    /// <summary>
    /// Create method to simulate raw data and send test order results via gRPC
    /// </summary>
    /// <seealso cref="Simulator.API.Protos.Query.RawDataQueryService.RawDataQueryServiceBase" />
    public class RawDataQueryGrpcService : RawDataQueryServiceBase
    {
        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawDataQueryGrpcService"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public RawDataQueryGrpcService(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Creates the and send test order.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="Grpc.Core.RpcException"></exception>
        /// <exception cref="Simulator.API.Protos.Query.RawResultItem.Status">
        /// Invalid TestOrderId format.
        /// or
        /// Failed to simulate raw data for Test Order ID {orderId}.
        /// </exception>
        public override async Task<GetTestOrderResultsResponse> CreateAndSendTestOrder(
             CreateAndSendTestOrderRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TestOrderId, out var orderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid TestOrderId format."));
            }

            var rawResultDto = await _mediator.Send(new SimulateRawDataCommand(orderId));

            if (rawResultDto == null || !rawResultDto.Results.Any())
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Failed to simulate raw data for Test Order ID {orderId}."));
            }

            var response = new GetTestOrderResultsResponse
            {
                TestOrderId = rawResultDto.TestOrderId.ToString(),
                Instrument = rawResultDto.Instrument,
                PerformedDate = Timestamp.FromDateTime(rawResultDto.PerformedDate.ToUniversalTime()),
                Results =
                {
                    rawResultDto.Results.Select(item => new RawResultItem
                    {
                        TestCode = item.TestCode,
                        Parameter = item.Parameter,
                        ValueNumeric = item.ValueNumeric ?? 0,
                        ValueText = item.ValueText ?? string.Empty,
                        Unit = item.Unit,
                        ReferenceRange = item.ReferenceRange,
                        Status = item.Status
                    })
                }
            };

            return response;
        }
        /// <summary>
        /// Gets the test order results.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="Grpc.Core.RpcException"></exception>
        /// <exception cref="Simulator.API.Protos.Query.RawResultItem.Status">
        /// Invalid TestOrderId format.
        /// or
        /// Test Order ID {orderId} not found in Simulator DB.
        /// </exception>
        public override async Task<GetTestOrderResultsResponse> GetTestOrderResults(
            GetTestOrderResultsRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TestOrderId, out var orderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid TestOrderId format."));
            }
            var dto = await _mediator.Send(new GetRawTestResultQuery(orderId));

            if (dto == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Test Order ID {orderId} not found in Simulator DB."));
            }
            var response = new GetTestOrderResultsResponse
            {
                TestOrderId = dto.TestOrderId.ToString(),
                Instrument = dto.Instrument,
                PerformedDate = Timestamp.FromDateTime(dto.PerformedDate.ToUniversalTime()),
                Results =
                {
                    dto.Results.Select(item => new RawResultItem
                    {
                        TestCode = item.TestCode,
                        Parameter = item.Parameter,
                        ValueNumeric = item.ValueNumeric ?? 0,
                        ValueText = item.ValueText ?? string.Empty,
                        Unit = item.Unit,
                        ReferenceRange = item.ReferenceRange,
                        Status = item.Status
                    })
                }
            };

            return response;
        }
    }
}