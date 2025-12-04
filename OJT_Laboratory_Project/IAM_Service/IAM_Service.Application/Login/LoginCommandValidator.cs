using FluentValidation;

namespace IAM_Service.Application.Login
{
    /// <summary>
    /// Validator for the LoginCommand to ensure valid input.
    /// </summary>
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        /// <summary>
        /// Constructor that sets up validation rules for the LoginCommand.
        /// </summary>
        /// <returns>A new instance of LoginCommandValidator.</returns>
        /// <exception cref="Exception">Thrown when the validation fails.</exception>
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password not null");
        }
    }
}
