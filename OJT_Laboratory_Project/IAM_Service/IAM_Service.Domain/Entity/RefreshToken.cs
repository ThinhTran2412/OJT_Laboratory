namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// create attribute for class RefreshToken 
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        /// <value>
        /// The expiry date.
        /// </value>
        public DateTime ExpiryDate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is revoked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is revoked; otherwise, <c>false</c>.
        /// </value>
        public bool IsRevoked { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User User { get; set; }
    }
}
