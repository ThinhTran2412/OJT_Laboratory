namespace IAM_Service.Application.DTOs.Privileges
{
    /// <summary>
    /// Represents a privilege data transfer object for API responses.
    /// Contains basic privilege information suitable for dropdowns and lists.
    /// </summary>
    public class PrivilegeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the privilege.
        /// </summary>
        public int PrivilegeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the privilege.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the privilege.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
