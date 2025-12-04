using MediatR;

/// <summary>
/// Command for creating a new user.
/// </summary>
namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// Create many propery same entity User
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;MediatR.Unit&gt;" />

    /// Command representing a request to create a new user with various details.
    /// </summary>
    public class CreateUserCommand : IRequest<Unit>
    {
        /// <summary> The full name of the user. </summary>

        public string FullName { get; set; } = string.Empty;
        /// <summary> The email address of the user. </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary> The phone number of the user. </summary>
        public string PhoneNumber { get; set; } = string.Empty;
        /// <summary> The identification number of the user. </summary>
        public string IdentifyNumber { get; set; } = string.Empty;
        /// <summary> The gender of the user. </summary>
        public string Gender { get; set; } = string.Empty;
        /// <summary> The age of the user. </summary>
        public int Age { get; set; }
        /// <summary> The address of the user. </summary>
        public string Address { get; set; } = string.Empty;
        /// <summary> The date of birth of the user. </summary>
        public DateOnly DateOfBirth { get; set; }
        
        /// <summary> The role ID to assign to the user. If null, will assign default Read-Only role. </summary>
        public int? RoleId { get; set; }
    }
}
