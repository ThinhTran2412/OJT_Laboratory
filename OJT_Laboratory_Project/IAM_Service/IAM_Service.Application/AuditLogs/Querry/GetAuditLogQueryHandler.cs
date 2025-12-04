using AutoMapper;
using IAM_Service.Application.DTOs;
using IAM_Service.Application.Interface.IAuditLogRepository;
using MediatR;

namespace IAM_Service.Application.AuditLogs.Querry
{
    /// <summary>
    /// Handle Get AuditLog Query
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.AuditLogs.Querry.GetAuditLogQuery, System.Collections.Generic.List&lt;IAM_Service.Application.DTOs.AuditLogDto&gt;&gt;" />
    public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, List<AuditLogDto>>
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IAuditLogRepository _repository;
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAuditLogQueryHandler"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="repository">The repository.</param>
        public GetAuditLogQueryHandler(IMapper mapper, IAuditLogRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<List<AuditLogDto>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
        {
            var auditLog = await _repository.GetAllAsync();
            return _mapper.Map<List<AuditLogDto>>(auditLog);
        }
    }
}
