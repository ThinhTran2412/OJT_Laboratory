namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// Join entity for the many-to-many relationship between Role and Privilege.
    /// The composite key (RoleId, PrivilegeId) is configured in DbContext.
    /// </summary>
    public class RolePrivilege
    {
        /// <summary>
        /// Foreign key to Role.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        public int RoleId { get; set; }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Foreign key to Privilege.
        /// </summary>
        /// <value>
        /// The privilege identifier.
        /// </value>
        public int PrivilegeId { get; set; }
        /// <summary>
        /// Gets or sets the privilege.
        /// </summary>
        /// <value>
        /// The privilege.
        /// </value>
        public Privilege Privilege { get; set; } = null!;
    }
}


