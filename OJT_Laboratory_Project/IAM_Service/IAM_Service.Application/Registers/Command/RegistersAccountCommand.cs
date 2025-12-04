using MediatR;

namespace IAM_Service.Application.Registers.Command
{
    public class RegistersAccountCommand : IRequest<Unit>
    {
        /// <summary> The full name of the user. </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary> The email address of the user. </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary> The identification number of the user. </summary>
        public string IdentifyNumber { get; set; } = string.Empty;

        /// <summary> The password of the user. </summary>
        public string Password {  get; set; } = string.Empty;
    }
}
