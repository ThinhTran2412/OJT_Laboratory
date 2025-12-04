using Laboratory_Service.Application.DTOs.TestResult;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Laboratory_Service.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Laboratory_Service.Application.Interface.IAiReviewService" />
    public class AiReviewService : IAiReviewService
    {
        /// <summary>
        /// The ml context
        /// </summary>
        private readonly MLContext _mlContext;
        /// <summary>
        /// The model
        /// </summary>
        private ITransformer? _model;
        /// <summary>
        /// The prediction engine
        /// </summary>
        private PredictionEngine<TestResultData, TestResultPrediction>? _predictionEngine;
        /// <summary>
        /// The label mapping
        /// </summary>
        private List<string>? _labelMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="AiReviewService"/> class.
        /// </summary>
        public AiReviewService()
        {
            _mlContext = new MLContext(seed: 0);
        }

        /// <summary>
        /// Trains the model asynchronous.
        /// </summary>
        /// <param name="testResults">The test results.</param>
        /// <returns></returns>
        public Task TrainModelAsync(IEnumerable<TestResult> testResults)
        {
            var mlData = testResults
                .Where(x => x.ValueNumeric.HasValue
                            && !string.IsNullOrWhiteSpace(x.Parameter)
                            && !string.IsNullOrWhiteSpace(x.Unit)
                            && !string.IsNullOrWhiteSpace(x.ResultStatus))
                .Select(x => new TestResultData
                {
                    ValueNumeric = (float)x.ValueNumeric.Value,
                    Parameter = x.Parameter!,
                    Unit = x.Unit!,
                    ResultStatus = x.ResultStatus
                })
                .ToList();

            if (!mlData.Any())
            {
                return Task.CompletedTask;
            }

            var data = _mlContext.Data.LoadFromEnumerable(mlData);

            // Pipeline ML
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(TestResultData.ResultStatus))
                .Append(_mlContext.Transforms.Text.FeaturizeText("ParameterFeats", nameof(TestResultData.Parameter)))
                .Append(_mlContext.Transforms.Text.FeaturizeText("UnitFeats", nameof(TestResultData.Unit)))
                .Append(_mlContext.Transforms.Concatenate("Features", "ValueNumeric", "ParameterFeats", "UnitFeats"))
                .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
                    new Microsoft.ML.Trainers.LbfgsMaximumEntropyMulticlassTrainer.Options
                    {
                        LabelColumnName = "Label",
                        FeatureColumnName = "Features",
                        MaximumNumberOfIterations = 50
                    }))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "Label"));

            try
            {
                _model = Task.Run(() => pipeline.Fit(data)).Result;

                try
                {
                    var transformedData = _model.Transform(data);
                    var labelColumn = transformedData.Schema["Label"];
                    
                    // Lấy key values từ metadata
                    if (labelColumn.HasKeyValues())
                    {
                        var keyValues = default(Microsoft.ML.Data.VBuffer<ReadOnlyMemory<char>>);
                        labelColumn.GetKeyValues(ref keyValues);
                        
                        _labelMapping = new List<string>();
                        foreach (var kv in keyValues.GetValues())
                        {
                            _labelMapping.Add(kv.ToString());
                        }
                    }
                    else
                    {
                        _labelMapping = mlData
                            .Select(x => x.ResultStatus)
                            .Distinct()
                            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                            .ToList();
                    }
                }
                catch
                {
                    _labelMapping = mlData
                        .Select(x => x.ResultStatus)
                        .Distinct()
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }

                _predictionEngine = _mlContext.Model.CreatePredictionEngine<TestResultData, TestResultPrediction>(_model);
            }
            catch
            {
                throw;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Predicts the asynchronous.
        /// </summary>
        /// <param name="testResult">The test result.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Model chưa được train.</exception>
        public Task<string> PredictAsync(TestResult testResult)
        {
            if (!_predictionEngine.IsNotNull())
            {
                throw new Exception("Model not be train.");
            }

            // Input chuẩn
            var input = new TestResultData
            {
                ValueNumeric = (float)(testResult.ValueNumeric ?? 0),
                Parameter = testResult.Parameter ?? "",
                Unit = testResult.Unit ?? ""
            };

            // Predict
            string predictedStatus;
            try
            {
                var pred = _predictionEngine.Predict(input);

                if (string.IsNullOrWhiteSpace(pred.PredictedResultStatus))
                {
                    if (pred.Score != null && pred.Score.Length > 0 && _labelMapping != null && _labelMapping.Count > 0)
                    {
                        var maxScoreIndex = Array.IndexOf(pred.Score, pred.Score.Max());
                        
                        if (maxScoreIndex >= 0 && maxScoreIndex < _labelMapping.Count)
                        {
                            predictedStatus = _labelMapping[maxScoreIndex];
                        }
                        else
                        {
                            predictedStatus = ApplyRule(testResult);
                        }
                    }
                    else
                    {
                        predictedStatus = ApplyRule(testResult);
                    }
                }
                else
                {
                    predictedStatus = pred.PredictedResultStatus;
                }
            }
            catch
            {
                predictedStatus = ApplyRule(testResult);
            }

            testResult.ResultStatus = predictedStatus;

            return Task.FromResult(predictedStatus);
        }

        /// <summary>
        /// Applies the rule.
        /// </summary>
        /// <param name="testResult">The test result.</param>
        /// <returns></returns>
        private string ApplyRule(TestResult testResult)
        {
            if (!testResult.ValueNumeric.HasValue || string.IsNullOrWhiteSpace(testResult.ReferenceRange))
            {
                return "Pending";
            }

            var parts = testResult.ReferenceRange.Split('-');
            
            if (parts.Length != 2
                || !double.TryParse(parts[0], out double low)
                || !double.TryParse(parts[1], out double high))
            {
                return "Unknown";
            }

            var value = testResult.ValueNumeric.Value;
            
            if (value < low) return "Low";
            if (value > high) return "High";
            
            return "Normal";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether [is not null].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <c>true</c> if [is not null] [the specified object]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNull<T>(this T? obj) => obj != null;
    }
}
