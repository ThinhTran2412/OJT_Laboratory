namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Request model for exporting test orders.
    /// If TestOrderIds is null or empty, exports all test orders from current month.
    /// </summary>
    public class ExportTestOrdersRequest
    {
        /// <summary>
        /// List of specific test order IDs to export.
        /// If null or empty, exports all test orders from current month (per policy).
        /// </summary>
        public List<Guid>? TestOrderIds { get; set; }

        /// <summary>
        /// Custom filename for the exported Excel file.
        /// If not provided, uses default naming: "Test Orders-{PatientName}-{Date}.xlsx"
        /// </summary>
        public string? FileName { get; set; }
    }
}