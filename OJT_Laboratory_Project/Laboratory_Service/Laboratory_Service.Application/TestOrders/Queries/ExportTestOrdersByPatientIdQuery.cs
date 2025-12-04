using MediatR;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Query to export test orders for a specific patient to Excel file.
    /// Used by regular users to export their own test orders.
    /// </summary>
    public class ExportTestOrdersByPatientIdQuery : IRequest<byte[]>
    {
        /// <summary>
        /// Patient ID to export test orders for
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Optional custom filename. If not provided, defaults to "Test Orders-{PatientName}-{ExportDate}.xlsx"
        /// </summary>
        public string? FileName { get; set; }

        public ExportTestOrdersByPatientIdQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}

