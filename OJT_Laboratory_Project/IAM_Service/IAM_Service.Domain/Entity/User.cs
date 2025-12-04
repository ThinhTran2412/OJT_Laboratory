namespace IAM_Service.Domain.Entity
{
    /// <summary>
    /// create many property for User
    /// Represents a user in the system with various personal and security details.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }
        /// <summary>
        /// The full name of the user.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// The email address of the user.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// The phone number of the user.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary>
        /// The identification number of the user.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; } = string.Empty;
        /// <summary>
        /// The gender of the user.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = string.Empty;
        /// <summary>
        /// The age of the user.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age { get; set; }
        /// <summary>
        /// The address of the user.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOnly DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the failed login attempts.
        /// </summary>
        /// <value>
        /// The failed login attempts.
        /// </value>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// Gets or sets the lockout end.
        /// </summary>
        /// <value>
        /// The lockout end.
        /// </value>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets the last failed login at.
        /// </summary>
        /// <value>
        /// The last failed login at.
        /// </value>
        public DateTime? LastFailedLoginAt { get; set; }

        /// <summary>
        /// Foreign key to the user's assigned role.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        public int? RoleId { get; set; }

        /// <summary>
        /// Navigation property to the user's role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public Role? Role { get; set; }

        /// <summary>
        /// Gets or sets the refresh tokens.
        /// </summary>
        /// <value>
        /// The refresh tokens.
        /// </value>
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        /// <summary>
        /// Gets or sets the user privileges.
        /// </summary>
        /// <value>
        /// The user privileges.
        /// </value>
        public ICollection<UserPrivilege> UserPrivileges { get; set; } = new List<UserPrivilege>();
    }
}
