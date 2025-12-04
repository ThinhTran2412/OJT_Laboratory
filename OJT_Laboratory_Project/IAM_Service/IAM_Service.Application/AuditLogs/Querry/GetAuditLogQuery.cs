using IAM_Service.Application.DTOs;
using MediatR;

namespace IAM_Service.Application.AuditLogs.Querry
{
    /// <summary>
    /// create class solve get AuditLogQuery
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Collections.Generic.List&lt;IAM_Service.Application.DTOs.AuditLogDto&gt;&gt;" />
    public class GetAuditLogQuery : IRequest<List<AuditLogDto>>
    {
    }
}
