using MediatR;
using System;
using System.Collections.Generic;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Query for exporting selected test orders or all orders from current month.
    /// Business rules:
    /// - If TestOrderIds is null/empty, export all test orders from current month
    /// - If TestOrderIds has values, export only those specific orders
    /// - Run By and Run On columns are empty if Status is not "Completed"
    /// </summary>
    public class ExportSelectedTestOrdersQuery : IRequest<byte[]>
    {
        /// <summary>
        /// Patient ID whose test orders will be exported
        /// </summary>
        public int PatientId { get; set; }

        /// <summary>
        /// Specific test order IDs to export. 
        /// Null or empty = export all orders from current month
        /// </summary>
        public List<Guid>? TestOrderIds { get; set; }

        /// <summary>
        /// Custom filename for the Excel file
        /// </summary>
        public string? FileName { get; set; }

        public ExportSelectedTestOrdersQuery(int patientId)
        {
            PatientId = patientId;
        }
    }
}