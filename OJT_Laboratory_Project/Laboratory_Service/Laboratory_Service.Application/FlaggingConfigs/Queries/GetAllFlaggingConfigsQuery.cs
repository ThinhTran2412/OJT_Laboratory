using Laboratory_Service.Application.DTOs.FlaggingConfig;
using MediatR;

namespace Laboratory_Service.Application.FlaggingConfigs.Queries
{
    public record GetAllFlaggingConfigsQuery : IRequest<List<FlaggingConfigUpsertItemDto>>;
}
