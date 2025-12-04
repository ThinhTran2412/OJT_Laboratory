using MediatR;

namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// Command dùng để cập nhật thông tin người dùng.
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;MediatR.Unit&gt;" />
    public class UpdateUserCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string? FullName { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string? Gender { get; set; }
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int? Age { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string? Address { get; set; }
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateOnly? DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the privilege ids.
        /// </summary>
        /// <value>
        /// The privilege ids.
        /// </value>
        public List<int>? PrivilegeIds { get; set; }
        /// <summary>
        /// Gets or sets the type of the action.
        /// </summary>
        /// <value>
        /// The type of the action.
        /// </value>
        public string? ActionType { get; set; }  // "update", "add", "reset"
    }

}
