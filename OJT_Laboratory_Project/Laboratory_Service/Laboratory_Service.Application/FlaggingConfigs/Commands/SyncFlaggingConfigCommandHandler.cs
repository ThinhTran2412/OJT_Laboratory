using System;
using System.Linq;
using Laboratory_Service.Application.DTOs.FlaggingConfig;
using System.Collections.Generic;
using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Laboratory_Service.Application.FlaggingConfigs.Commands;

public class SyncFlaggingConfigCommandHandler : IRequestHandler<SyncFlaggingConfigCommand, SyncFlaggingConfigResultDto>
{
    private readonly IFlaggingConfigRepository _flaggingConfigRepository;
    private readonly ILogger<SyncFlaggingConfigCommandHandler> _logger;
    private readonly ITestResultRepository _testResultRepository;
    private readonly IFlaggingService _flaggingService;

    public SyncFlaggingConfigCommandHandler(
        IFlaggingConfigRepository flaggingConfigRepository,
        ILogger<SyncFlaggingConfigCommandHandler> logger,
        ITestResultRepository testResultRepository,
        IFlaggingService flaggingService)
    {
        _flaggingConfigRepository = flaggingConfigRepository;
        _logger = logger;
        _testResultRepository = testResultRepository;
        _flaggingService = flaggingService;
    }

    public async Task<SyncFlaggingConfigResultDto> Handle(SyncFlaggingConfigCommand request, CancellationToken cancellationToken)
    {
        if (request.Configs == null || request.Configs.Count == 0)
        {
            _logger.LogWarning("SyncFlaggingConfigCommand received empty payload.");

            return new SyncFlaggingConfigResultDto
            {
                Message = "No flagging configs to sync."
            };
        }

        var normalizedItems = NormalizePayload(request.Configs);
        var affectedTestCodes = ExtractTestCodes(normalizedItems);

        await _flaggingConfigRepository.UpsertConfigsAsync(normalizedItems, cancellationToken);

        await RecalculateFlagsAsync(affectedTestCodes, cancellationToken);

        var message = "Synced flagging configs.";
        _logger.LogInformation(message);

        return new SyncFlaggingConfigResultDto
        {
            Message = message
        };
    }

    private static IReadOnlyCollection<FlaggingConfigUpsertItemDto> NormalizePayload(IEnumerable<FlaggingConfigUpsertItemDto> rawItems)
    {
        return rawItems
            .Where(item => !string.IsNullOrWhiteSpace(item.TestCode))
            .Select(item => new FlaggingConfigUpsertItemDto
            {
                TestCode = item.TestCode.Trim(),
                ParameterName = string.IsNullOrWhiteSpace(item.ParameterName) ? null : item.ParameterName.Trim(),
                Description = item.Description?.Trim(),
                Unit = string.IsNullOrWhiteSpace(item.Unit) ? null : item.Unit.Trim(),
                Gender = string.IsNullOrWhiteSpace(item.Gender) ? null : item.Gender.Trim(),
                Min = item.Min,
                Max = item.Max,
                IsActive = item.IsActive,
                EffectiveDate = item.EffectiveDate
            })
            .ToList();
    }

    private static IReadOnlyCollection<string> ExtractTestCodes(IEnumerable<FlaggingConfigUpsertItemDto> configs)
    {
        return configs
            .Where(item => !string.IsNullOrWhiteSpace(item.TestCode))
            .Select(item => item.TestCode.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();
    }

    private async Task RecalculateFlagsAsync(IReadOnlyCollection<string> testCodes, CancellationToken cancellationToken)
    {
        if (testCodes == null || testCodes.Count == 0)
        {
            return;
        }

        var normalizedCodes = testCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();

        if (normalizedCodes.Count == 0)
        {
            return;
        }

        var affectedResults = await _testResultRepository.GetByTestCodesAsync(normalizedCodes, cancellationToken);
        if (!affectedResults.Any())
        {
            _logger.LogInformation("No test results need recalculation for test codes: {TestCodes}", string.Join(", ", normalizedCodes));
            return;
        }

        var updates = new List<TestResult>();
        var now = DateTime.UtcNow;

        foreach (var result in affectedResults)
        {
            if (!result.ValueNumeric.HasValue)
            {
                continue;
            }

            var gender = result.TestOrder?.MedicalRecord?.Patient?.Gender;
            var newFlag = await _flaggingService.CalculateFlagAsync(result.TestCode, result.ValueNumeric, gender, cancellationToken);

            if (string.Equals(result.Flag, newFlag, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            updates.Add(new TestResult
            {
                TestResultId = result.TestResultId,
                Flag = newFlag,
                FlaggedAt = now,
                FlaggedBy = null
            });
        }

        if (!updates.Any())
        {
            _logger.LogInformation("Recalculation resulted in no changes for test codes: {TestCodes}", string.Join(", ", normalizedCodes));
            return;
        }

        await _testResultRepository.UpdateFlagsAsync(updates, cancellationToken);
        _logger.LogInformation("Recalculated flags for {Count} test results (test codes: {TestCodes}).", updates.Count, string.Join(", ", normalizedCodes));
    }
}

