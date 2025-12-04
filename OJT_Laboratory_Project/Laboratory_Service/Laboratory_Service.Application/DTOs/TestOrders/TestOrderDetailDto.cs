namespace Laboratory_Service.Application.DTOs.TestOrders
{
    public class TestOrderDetailDto
    {
        /// <summary>Unique identifier of the test order.</summary>
        public Guid TestOrderId { get; set; }

        /// <summary>Foreign key to patient.</summary>
        public int PatientId { get; set; }

        // Patient Information (from MedicalRecord)
        /// <summary>Name of the patient.</summary>
        public string PatientName { get; set; } = string.Empty;

        /// <summary>Age of the patient.</summary>
        public int Age { get; set; }

        /// <summary>Gender of the patient.</summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>Phone number of the patient.</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>Identification number of the patient.</summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        // Test Order Details
        /// <summary>Status of the test order (e.g., Pending, Cancelled, Completed).</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Date when the test order was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Date when the test was run. Null if test has not been run yet.</summary>
        public DateTime? RunDate { get; set; }

        // Removed CreatedByUserName and RunByUserName in new model

        /// <summary>Type of test for this order.</summary>
        public string TestType { get; set; } = string.Empty;

        /// <summary>Priority of the test order.</summary>
        public string? Priority { get; set; }

        /// <summary>Note for the test order.</summary>
        public string? Note { get; set; }

        /// <summary>Test results for this order.</summary>
        public List<TestResultDto> TestResults { get; set; } = new();

        /// <summary>Optional message (e.g., No Test Results).</summary>
        public string? Message { get; set; }
    }
}
