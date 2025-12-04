using Laboratory_Service.Application.DTOs.Pagination;
using Laboratory_Service.Application.DTOs.TestOrders;
using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    public class GetTestOrdersQuery : IRequest<PagedResponse<TestOrderListItemDto>>
    {
        /// <summary>
        /// Keyword to search by Patient Name, Phone Number, Status, or User names (case-insensitive).
        /// </summary>
        public string? Search { get; set; }

        /// <summary>Page number (1-based). Default is 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Page size. Default is 10.</summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort column: id | patientName | age | gender | phoneNumber | status | createdDate | runDate.
        /// Default is createdDate when not provided.
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Sort direction. Default is true (descending) for createdDate to show most recent first.
        /// </summary>
        public bool SortDesc { get; set; } = true;

        /// <summary>
        /// Filter by status (e.g., Pending, Cancelled, Completed).
        /// If not provided, all statuses are returned.
        /// </summary>
        public string? Status { get; set; }
    }
}
