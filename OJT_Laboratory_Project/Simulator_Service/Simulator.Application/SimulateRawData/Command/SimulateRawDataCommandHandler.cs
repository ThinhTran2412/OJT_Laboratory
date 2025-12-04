using MediatR;
using Simulator.Application.Constants;
using Simulator.Application.DTOs;
using Simulator.Application.Interface;

namespace Simulator.Application.SimulateRawData.Command
{
    /// <summary>
    /// Handles the simulate raw data command
    /// </summary>
    public class SimulateRawDataCommandHandler 
        : IRequestHandler<SimulateRawDataCommand, RawTestResultDTO>
    {
        private readonly Random _random = new Random();
        private readonly IRawTestResultRepository _repository;

        private const double DeviationFactor = 0.15;

        public SimulateRawDataCommandHandler(IRawTestResultRepository repository)
        {
            _repository = repository;
        }

        // ============================
        // GIẢI PHÁP 2: CHIA THEO NHÓM
        // ============================
        private double GenerateRandomValue(TestParameter param)
        {
            double range = param.MaxValue - param.MinValue;

            double effectiveMin = param.MinValue - range * DeviationFactor;
            double effectiveMax = param.MaxValue + range * DeviationFactor;

            effectiveMin = Math.Max(0, effectiveMin);

            double rawValue = _random.NextDouble() * (effectiveMax - effectiveMin) + effectiveMin;

            // Nhóm 1: WBC - PLT → số nguyên
            if (param.Abbreviation is "WBC" or "PLT")
                return Math.Round(rawValue, 0);

            // Nhóm 2: RBC - HGB - HCT → lấy 2 số thập phân
            if (param.Abbreviation is "RBC" or "HGB" or "HCT")
                return Math.Round(rawValue, 2);

            // Nhóm 3: mặc định → 1 số thập phân
            return Math.Round(rawValue, 1);
        }

        // ============================
        // GIẢI PHÁP 2: CHIA THEO NHÓM
        // ============================
        private string GetStatus(double value, TestParameter param)
        {
            // Nhóm 1: WBC - PLT (biên độ rộng → +/- 10%)
            if (param.Abbreviation is "WBC" or "PLT")
            {
                if (value < param.MinValue * 0.90) return "Low";
                if (value > param.MaxValue * 1.10) return "High";
                return "Normal";
            }

            // Nhóm 2: RBC - HGB - HCT (biên độ chặt → +/- 5%)
            if (param.Abbreviation is "RBC" or "HGB" or "HCT")
            {
                if (value < param.MinValue * 0.95) return "Low";
                if (value > param.MaxValue * 1.05) return "High";
                return "Normal";
            }

            // Nhóm 3: Các test khác → +/- 7%
            if (value < param.MinValue * 0.93) return "Low";
            if (value > param.MaxValue * 1.07) return "High";
            return "Normal";
        }

        public async Task<RawTestResultDTO> Handle(SimulateRawDataCommand request, CancellationToken cancellationToken)
        {
            Guid testOrderId = request.TestOrderId;

            string instrument = $"Sysmex-XN-{_random.Next(100, 999)}";
            DateTime performedDate = DateTime.UtcNow.AddMinutes(-_random.Next(1, 60));

            var rawResultItems = new List<RawResultItemDTO>();

            foreach (var param in TestParameterData.Parameters)
            {
                double value = GenerateRandomValue(param);
                string status = GetStatus(value, param);

                rawResultItems.Add(new RawResultItemDTO
                {
                    TestCode = param.Abbreviation,
                    Parameter = param.Parameter,
                    ValueNumeric = value,
                    ValueText = value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Unit = param.Unit,
                    ReferenceRange = param.NormalRange,
                    Status = status
                });
            }

            var rawTestResultDto = new RawTestResultDTO
            {
                TestOrderId = testOrderId,
                Instrument = instrument,
                PerformedDate = performedDate,
                Results = rawResultItems
            };

            await _repository.AddRangeAsync(new List<RawTestResultDTO> { rawTestResultDto });

            return rawTestResultDto;
        }
    }
}
