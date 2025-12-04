namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// create attribute for AuditLog
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        /// <value>
        /// The user email.
        /// </value>
        public required string UserEmail { get; set; }
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        public required string EntityName { get; set; }
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public required string Action { get; set; }
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public required DateTime Timestamp { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Gets or sets the changes.
        /// </summary>
        /// <value>
        /// The changes.
        /// </value>
        public required string Changes { get; set; }
    }
}
