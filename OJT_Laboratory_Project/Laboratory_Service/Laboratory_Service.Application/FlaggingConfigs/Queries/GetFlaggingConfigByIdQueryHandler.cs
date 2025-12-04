using Laboratory_Service.Application.DTOs.FlaggingConfig;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.FlaggingConfigs.Queries
{
    public class GetFlaggingConfigByIdQueryHandler : IRequestHandler<GetFlaggingConfigByIdQuery, FlaggingConfigUpsertItemDto?>
    {
        private readonly IFlaggingConfigRepository _repository;

        public GetFlaggingConfigByIdQueryHandler(IFlaggingConfigRepository repository)
        {
            _repository = repository;
        }

        public async Task<FlaggingConfigUpsertItemDto?> Handle(GetFlaggingConfigByIdQuery request, CancellationToken cancellationToken)
        {
            var fc = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (fc == null) return null;

            return new FlaggingConfigUpsertItemDto
            {
                TestCode = fc.TestCode,
                ParameterName = fc.ParameterName,
                Description = fc.Description,
                Unit = fc.Unit,
                Gender = fc.Gender,
                Min = fc.Min,
                Max = fc.Max,
                IsActive = fc.IsActive,
                EffectiveDate = fc.EffectiveDate
            };
        }
    }
}
