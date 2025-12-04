using Laboratory_Service.Application.Interface;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.Services
{
    /// <summary>
    /// Service for calculating flags (Low/Normal/High) for test results based on FlaggingConfig.
    /// </summary>
    public class FlaggingService : IFlaggingService
    {
        private readonly IFlaggingConfigRepository _configRepo;
        private readonly ILogger<FlaggingService> _logger;

        public FlaggingService(
            IFlaggingConfigRepository configRepo,
            ILogger<FlaggingService> logger)
        {
            _configRepo = configRepo;
            _logger = logger;
        }

        /// <summary>
        /// Calculates the flag (Low, Normal, or High) based on value and FlaggingConfig.
        /// Logic: value < Min → "Low", Min ≤ value ≤ Max → "Normal", value > Max → "High"
        /// </summary>
        public async Task<string> CalculateFlagAsync(string testCode, double? value, string? gender, CancellationToken cancellationToken = default)
        {
            // Nếu value null, trả về Normal
            if (!value.HasValue)
            {
                return "Normal";
            }

            // Lấy FlaggingConfig từ database
            var config = await _configRepo.GetActiveConfigAsync(testCode, gender, cancellationToken);
            if (config == null)
            {
                _logger.LogWarning("FlaggingConfig not found for TestCode: {TestCode}, Gender: {Gender}. Returning Normal.", testCode, gender);
                return "Normal";
            }

            // So sánh value với Min/Max
            if (value < config.Min)
            {
                return "Low";
            }

            if (value > config.Max)
            {
                return "High";
            }

            return "Normal";
        }
    }
}

