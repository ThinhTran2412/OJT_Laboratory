namespace IAM_Service.Application.DTOs.Pagination
{
    /// <summary>
    /// API-facing paged response wrapper returned to clients.
    /// Contains a read-only list of items and paging metadata.
    /// </summary>
    public class PagedResponse<T>
    {
        /// <summary>Items in the current page (read-only from API perspective).</summary>
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
        /// <summary>Total number of records that match the query (without paging).</summary>
        public int Total { get; set; }
        /// <summary>Current page number (1-based).</summary>
        public int Page { get; set; }
        /// <summary>Number of items per page.</summary>
        public int PageSize { get; set; }
        /// <summary>Optional message (e.g., "No Data" when the page is empty).</summary>
        public string? Message { get; set; }
    }
}


