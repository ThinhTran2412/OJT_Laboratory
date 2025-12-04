/// <summary>
/// Configuration options for account lockout.
/// </summary>
namespace IAM_Service.Application.Common.Security
{   
    /// <summary>
    /// Configuration options for account lockout.
    /// </summary>
    public class LockoutOptions
    {
        /// <summary>
        /// Indicates whether lockout is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// The maximum number of failed access attempts before lockout.
        /// </summary>
        public int MaxFailedAccessAttempts { get; set; } = 5;
        /// <summary>
        /// The duration in minutes for which the account remains locked out.
        /// </summary>
        public int LockoutMinutes { get; set; } = 15;
    }
}


