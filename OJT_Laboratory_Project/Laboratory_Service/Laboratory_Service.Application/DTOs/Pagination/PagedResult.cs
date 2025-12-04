
namespace Laboratory_Service.Application.DTOs.Pagination
{
    public class PagedResult<T>
    {
        /// <summary>Materialized items for the current page.</summary>
        public List<T> Items { get; set; } = new List<T>();
        /// <summary>Total number of records that match the query (without paging).</summary>
        public int Total { get; set; }
        /// <summary>Current page (1-based).</summary>
        public int Page { get; set; }
        /// <summary>Page size.</summary>
        public int PageSize { get; set; }
    }
}
