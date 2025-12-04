
using Microsoft.ML.Data;
namespace Laboratory_Service.Application.DTOs.TestResult
{
    /// <summary>
    /// Create TestResultData
    /// </summary>
    public class TestResultData
    {
        /// <summary>
        /// The value numeric
        /// </summary>
        [LoadColumn(0)]
        public float ValueNumeric;

        /// <summary>
        /// The parameter
        /// </summary>
        [LoadColumn(1)]
        public string Parameter;

        /// <summary>
        /// The unit
        /// </summary>
        [LoadColumn(2)]
        public string Unit;

        /// <summary>
        /// The result status
        /// </summary>
        [LoadColumn(3)]
        public string ResultStatus; // Label
    }
}
