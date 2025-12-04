namespace IAM_Service.Application.DTOs
{
    public class UserDTO
    {
        /// <summary>
        /// Unique identifier of the user.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Full name of the user.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the user.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number of the user.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// National ID or identification number of the user.
        /// </summary>
        /// <value>
        /// The identify number.
        /// </value>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gender of the user (e.g., Male, Female, Other).
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Age of the user.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age { get; set; }

        /// <summary>
        /// Residential address of the user.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Date of birth of the user.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOnly DateOfBirth { get; set; }
    }
}
