namespace Laboratory_Service.Domain.Entity
{
    /// <summary>
    /// Represents user information from IAM Service
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// User ID from IAM Service
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User's full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// User's identification number
        /// </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// User's gender
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// User's age
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// User's address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// User's date of birth
        /// </summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// User's role ID
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// User's role name
        /// </summary>
        public string? RoleName { get; set; }
    }
}
