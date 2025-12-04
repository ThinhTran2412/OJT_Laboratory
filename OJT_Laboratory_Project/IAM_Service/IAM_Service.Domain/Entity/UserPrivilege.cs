namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// Join entity for the many-to-many relationship between User and Privilege.
    /// The composite key (UserIdId, PrivilegeId) is configured in DbContext.
    /// </summary>
    public class UserPrivilege
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the privilege identifier.
        /// </summary>
        /// <value>
        /// The privilege identifier.
        /// </value>
        public int PrivilegeId { get; set; }

        // Navigation properties
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; }
        /// <summary>
        /// Gets or sets the privilege.
        /// </summary>
        /// <value>
        /// The privilege.
        /// </value>
        public Privilege Privilege { get; set; }
    }
}