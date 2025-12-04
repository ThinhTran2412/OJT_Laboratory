
namespace IAM_Service.Application.DTOs.Roles
{
    /// <summary>
    /// create attribute for class RoleCreateDto
    /// </summary>
    public class RoleCreateDto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the privilege ids.
        /// </summary>
        /// <value>
        /// The privilege ids.
        /// </value>
        public List<int>? PrivilegeIds { get; set; }
    }
}
