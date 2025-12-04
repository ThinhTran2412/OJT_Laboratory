using MediatR;
using IAM_Service.Application.DTOs.Privileges;

namespace IAM_Service.Application.Privileges.Query
{
    /// <summary>
    /// Represents a query to retrieve all privileges in the system.
    /// This query is typically used to populate dropdown filters.
    /// </summary>
    public class GetPrivilegesQuery : IRequest<List<PrivilegeDto>>
    {
        // No parameters needed - returns all privileges
    }
}
