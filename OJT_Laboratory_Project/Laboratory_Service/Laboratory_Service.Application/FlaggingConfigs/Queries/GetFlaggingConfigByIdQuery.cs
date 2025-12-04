using Laboratory_Service.Application.DTOs.FlaggingConfig;
using MediatR;

namespace Laboratory_Service.Application.FlaggingConfigs.Queries
{
    public class GetFlaggingConfigByIdQuery : IRequest<FlaggingConfigUpsertItemDto?>
    {
        public int Id { get; set; }

        public GetFlaggingConfigByIdQuery(int id)
        {
            Id = id;
        }
    }
}
