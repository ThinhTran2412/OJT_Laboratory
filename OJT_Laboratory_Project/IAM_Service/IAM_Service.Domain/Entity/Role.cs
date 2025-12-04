namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// Represents a security role which groups privileges.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
       

        public int RoleId { get; set; }
        /// <summary>
        /// Human-readable name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Stable code used in logic/configuration.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; } = string.Empty;
        /// <summary>
        /// Description shown to administrators.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Many-to-many link to privileges through the join entity.
        /// </summary>
        /// <value>
        /// The role privileges.
        /// </value>
        public ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
    }
}


