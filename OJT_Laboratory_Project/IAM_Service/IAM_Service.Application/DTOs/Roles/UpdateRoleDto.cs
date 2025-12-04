using System.ComponentModel.DataAnnotations;

namespace IAM_Service.Application.DTOs.Roles
{
    /// <summary>
    /// DTO for updating an existing role.
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of privilege IDs associated with this role.
        /// If empty, the role will have no privileges.
        /// </summary>
        public List<int> PrivilegeIds { get; set; } = new List<int>();
        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        [Required(ErrorMessage = "Role code is required.")]
        [StringLength(50, ErrorMessage = "Code cannot be longer than 50 characters.")]
        public string Code { get; set; } = string.Empty; // ✅ thêm dòng này
    }
}
