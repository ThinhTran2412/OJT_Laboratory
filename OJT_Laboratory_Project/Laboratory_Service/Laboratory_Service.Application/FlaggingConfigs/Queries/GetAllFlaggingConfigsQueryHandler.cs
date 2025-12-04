using Laboratory_Service.Application.DTOs.FlaggingConfig;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.FlaggingConfigs.Queries
{
    public class GetAllFlaggingConfigsQueryHandler : IRequestHandler<GetAllFlaggingConfigsQuery, List<FlaggingConfigUpsertItemDto>>
    {
        private readonly IFlaggingConfigRepository _repository;

        public GetAllFlaggingConfigsQueryHandler(IFlaggingConfigRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FlaggingConfigUpsertItemDto>> Handle(GetAllFlaggingConfigsQuery request, CancellationToken cancellationToken)
        {
            var configs = await _repository.GetAllActiveConfigsAsync(cancellationToken);

            return configs.Select(fc => new FlaggingConfigUpsertItemDto
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
            }).ToList();
        }
    }
}
