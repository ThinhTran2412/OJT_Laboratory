using Laboratory_Service.Domain.Entity;

namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Service interface for calculating flags (Low/Normal/High) for test results based on FlaggingConfig.
    /// </summary>
    public interface IFlaggingService
    {
        /// <summary>
        /// Calculates the flag (Low, Normal, or High) for a test result based on its value and the flagging configuration.
        /// </summary>
        /// <param name="testCode">The test code (e.g., "WBC", "Hb", "RBC").</param>
        /// <param name="value">The numeric value of the test result.</param>
        /// <param name="gender">The gender of the patient ("Male", "Female", or null).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The calculated flag: "Low", "Normal", or "High". Returns "Normal" if config not found or value is null.</returns>
        Task<string> CalculateFlagAsync(string testCode, double? value, string? gender, CancellationToken cancellationToken = default);
    }
}

