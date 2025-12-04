using Microsoft.ML.Data;

namespace Laboratory_Service.Application.DTOs.TestResult
{
    /// <summary>
    /// Create TestResultPrediction
    /// </summary>
    public class TestResultPrediction
    {
        /// <summary>
        /// The predicted result status
        /// </summary>
        [ColumnName("PredictedLabel")]
        public string PredictedResultStatus;

        // Scores từ multiclass classification
        /// <summary>
        /// The score
        /// </summary>
        [ColumnName("Score")]
        public float[]? Score;
    }

}
