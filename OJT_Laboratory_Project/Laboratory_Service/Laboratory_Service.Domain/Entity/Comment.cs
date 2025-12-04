namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets or sets the comment identifier.
        /// </summary>
        /// <value>
        /// The comment identifier.
        /// </value>
        public int CommentId { get; set; }

        /// <summary>
        /// Gets or sets the test order identifier.
        /// </summary>
        /// <value>
        /// The test order identifier.
        /// </value>
        public Guid? TestOrderId { get; set; }
        /// <summary>
        /// Gets or sets the test result identifier.
        /// </summary>
        /// <value>
        /// The test result identifier.
        /// </value>
        public int? TestResultId { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public string CreatedBy { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public string? ModifiedBy { get; set; }
        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// Gets or sets the deleted by.
        /// </summary>
        /// <value>
        /// The deleted by.
        /// </value>
        public string? DeletedBy { get; set; }
        /// <summary>
        /// Gets or sets the deleted date.
        /// </summary>
        /// <value>
        /// The deleted date.
        /// </value>
        public DateTime? DeletedDate { get; set; }
    }
}

