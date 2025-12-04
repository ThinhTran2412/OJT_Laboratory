using MediatR;

namespace Laboratory_Service.Application.Test_Result.Queries
{
    /// <summary>
    /// Query to export test results to PDF for printing.
    /// Business rules:
    /// - TestOrder must exist and not be deleted
    /// - TestOrder Status must be "Completed" or "Đã hoàn thành"
    /// - File naming: "Chi tiết-Tên bệnh nhân-Ngày in.pdf" or custom name
    /// </summary>
    public class ExportTestResultsToPdfQuery : IRequest<byte[]>
    {
        /// <summary>
        /// Test Order ID to export
        /// </summary>
        public Guid TestOrderId { get; set; }

        /// <summary>
        /// Optional custom filename. If not provided, defaults to "Chi tiết-{PatientName}-{PrintDate}.pdf"
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportTestResultsToPdfQuery"/> class.
        /// </summary>
        /// <param name="testOrderId">The test order identifier.</param>
        public ExportTestResultsToPdfQuery(Guid testOrderId)
        {
            TestOrderId = testOrderId;
        }
    }
}