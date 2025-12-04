using IAM_Service.Application.DTOs.Privileges;

namespace IAM_Service.Application.DTOs.Roles
{
    /// <summary>
    /// Lightweight projection of Role used in list screens.
    /// </summary>
    public class RoleListItemDto
    {
        /// <summary>Unique identifier of the role.</summary>
        public int RoleId { get; set; }
        /// <summary>Human-readable role name.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Stable code of the role (used for system logic).</summary>
        public string Code { get; set; } = string.Empty;
        /// <summary>Description of the role.</summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>List of full privilege information belonging to this role.</summary>
        public IReadOnlyList<PrivilegeDto> Privileges { get; set; } = new List<PrivilegeDto>();
    }
}


