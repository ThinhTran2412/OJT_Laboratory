namespace IAM_Service.Domain.Entity
{
    public class PasswordReset
    {

        /// <summary>
        /// Primary key for the reset token.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the user who owns this token.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The reset token string.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the token expires.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Indicates whether this token has been used.
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Navigation property to the user.
        /// </summary>
        public User User { get; set; }
    }
}
