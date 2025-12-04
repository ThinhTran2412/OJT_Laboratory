namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Create attribute for class EventLog
    /// </summary>
    public class EventLog
    {
        /// <summary>
        /// Gets or sets the event log identifier.
        /// </summary>
        /// <value>
        /// The event log identifier.
        /// </value>
        public Guid EventLogId { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        /// <value>
        /// The event identifier.
        /// </value>
        public string EventId { get; set; } = default!;
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; } = default!;
        /// <summary>
        /// Gets or sets the event log message.
        /// </summary>
        /// <value>
        /// The event log message.
        /// </value>
        public string EventLogMessage { get; set; } = default!;
        /// <summary>
        /// Gets or sets the name of the operator.
        /// </summary>
        /// <value>
        /// The name of the operator.
        /// </value>
        public string OperatorName { get; set; } = default!;
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public string EntityType { get; set; } = default!;
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public Guid EntityId { get; set; }
        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>
        /// The created on.
        /// </value>
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
