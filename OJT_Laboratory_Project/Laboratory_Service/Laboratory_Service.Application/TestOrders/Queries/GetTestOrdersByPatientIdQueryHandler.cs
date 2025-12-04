using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Handler for getting test orders by patient ID.
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Queries.GetTestOrdersByPatientIdQuery, System.Collections.Generic.IEnumerable&lt;Laboratory_Service.Application.DTOs.TestOrders.TestOrderListItemDto&gt;&gt;" />
    public class GetTestOrdersByPatientIdQueryHandler : IRequestHandler<GetTestOrdersByPatientIdQuery, IEnumerable<TestOrderListItemDto>>
    {
        /// <summary>
        /// The test order repository
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestOrdersByPatientIdQueryHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepository">The test order repository.</param>
        public GetTestOrdersByPatientIdQueryHandler(ITestOrderRepository testOrderRepository)
        {
            _testOrderRepository = testOrderRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<IEnumerable<TestOrderListItemDto>> Handle(GetTestOrdersByPatientIdQuery request, CancellationToken cancellationToken)
        {
            // 1) Query repository: get all test orders for the patient
            var orders = await _testOrderRepository.GetAllByPatientIdAsync(request.PatientId, cancellationToken);

            // 2) Map entities -> DTOs. We avoid exposing domain navigation directly to API.
            var dtoItems = orders.Select(order => new TestOrderListItemDto
            {
                TestOrderId = order.TestOrderId,
                OrderCode = order.OrderCode ?? string.Empty,
                PatientName = order.MedicalRecord?.Patient?.FullName ?? string.Empty,
                Age = order.MedicalRecord?.Patient?.Age ?? 0,
                Gender = order.MedicalRecord?.Patient?.Gender ?? string.Empty,
                PhoneNumber = order.MedicalRecord?.Patient?.PhoneNumber ?? string.Empty,
                Status = order.Status,
                Priority = order.Priority ?? "Normal",
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                RunDate = order.RunDate,
                RunBy = order.RunBy,
                // Format test results if available
                TestResults = order.TestResults != null && order.TestResults.Any()
                    ? string.Join(", ", order.TestResults.Select(tr => 
                        $"{tr.Parameter}: {tr.ValueText ?? tr.ValueNumeric?.ToString() ?? "N/A"} {tr.Unit ?? ""}"))
                    : null
            }).ToList();

            // 3) Return list of DTOs
            return dtoItems;
        }
    }
}

