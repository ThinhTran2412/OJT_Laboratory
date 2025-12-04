namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// Represents an atomic permission that can be granted to a role.
    /// </summary>
    public class Privilege
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        /// <value>
        /// The privilege identifier.
        /// </value>
        public int PrivilegeId { get; set; }
        /// <summary>
        /// Display name of the privilege.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Optional description for documentation.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Many-to-many link to roles through the join entity.
        /// </summary>
        /// <value>
        /// The role privileges.
        /// </value>
        public ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
        /// <summary>
        /// Gets or sets the user privileges.
        /// </summary>
        /// <value>
        /// The user privileges.
        /// </value>
        public ICollection<UserPrivilege> UserPrivileges { get; set; } = new List<UserPrivilege>();

    }
}


