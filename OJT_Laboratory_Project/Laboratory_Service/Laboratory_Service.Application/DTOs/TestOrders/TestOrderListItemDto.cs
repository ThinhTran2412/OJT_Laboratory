namespace Laboratory_Service.Application.DTOs.TestOrders
{
    public class TestOrderListItemDto
    {
        /// <summary>Unique identifier of the test order.</summary>
        public Guid TestOrderId { get; set; }

        /// <summary>Order code of the test order.</summary>
        public string OrderCode { get; set; } = string.Empty;

        /// <summary>Name of the patient.</summary>
        public string PatientName { get; set; } = string.Empty;

        /// <summary>Age of the patient.</summary>
        public int Age { get; set; }

        /// <summary>Gender of the patient.</summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>Phone number of the patient.</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>Status of the test order (e.g., Pending, Cancelled, Completed).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Priority of the test order.</summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>Note for the test order.</summary>
        public string? Note { get; set; }

        /// <summary>Date when the test order was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Date when the test was run. Null if test has not been run yet.</summary>
        public DateTime? RunDate { get; set; }

        /// <summary>User ID who ran the test.</summary>
        public int? RunBy { get; set; }

        /// <summary>Test results as formatted string (if available).</summary>
        public string? TestResults { get; set; }

        // Removed CreatedByUserName and RunByUserName in new model
    }
}
