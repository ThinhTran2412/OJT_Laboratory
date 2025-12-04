using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Application.DTOs.TestOrders;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Handler for getting paginated list of test orders
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.TestOrders.Queries.GetTestOrdersQuery, Laboratory_Service.Application.DTOs.Pagination.PagedResponse&lt;Laboratory_Service.Application.DTOs.TestOrders.TestOrderListItemDto&gt;&gt;" />
    public class GetTestOrdersQueryHandler: IRequestHandler<GetTestOrdersQuery, PagedResponse<TestOrderListItemDto>>
    {
        /// <summary>
        /// The test order repository
        /// </summary>
        private readonly ITestOrderRepository _testOrderRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTestOrdersQueryHandler"/> class.
        /// </summary>
        /// <param name="testOrderRepository">The test order repository.</param>
        public GetTestOrdersQueryHandler(ITestOrderRepository testOrderRepository)
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
        public async Task<PagedResponse<TestOrderListItemDto>> Handle(GetTestOrdersQuery request, CancellationToken cancellationToken)
        {
            // 1) Query repository: this runs at database level (EF Core -> SQL), not in-memory.
            //    It applies search, status filter, sorting, and only then paging for efficiency.
            var result = await _testOrderRepository.GetTestOrdersAsync(
                request.Search,
                request.Page,
                request.PageSize,
                request.SortBy,
                request.SortDesc,
                request.Status,
                cancellationToken);

            // 2) Map entities -> DTOs. We avoid exposing domain navigation directly to API.
            var dtoItems = new List<TestOrderListItemDto>();

            foreach (var order in result.Items)
            {
                dtoItems.Add(new TestOrderListItemDto
                {
                    TestOrderId = order.TestOrderId,
                    PatientName = order.MedicalRecord?.Patient?.FullName ?? string.Empty,
                    Age = order.MedicalRecord?.Patient?.Age ?? 0,
                    Gender = order.MedicalRecord?.Patient?.Gender ?? string.Empty,
                    PhoneNumber = order.MedicalRecord?.Patient?.PhoneNumber ?? string.Empty,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    RunDate = order.RunDate
                });
            }

            // 3) Return a page wrapper with metadata (Total/Page/PageSize) and a friendly message when empty.
            return new PagedResponse<TestOrderListItemDto>
            {
                Items = dtoItems,
                Total = result.Total,
                Page = result.Page,
                PageSize = result.PageSize,
                Message = dtoItems.Count == 0 ? "No Data" : null
            };
        }
    }
}
