using FluentValidation;

namespace IAM_Service.Application.Registers.Command
{
    /// <summary>
    /// create validate for RegistersAccountCommand
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;IAM_Service.Application.Registers.Command.RegistersAccountCommand&gt;" />
    public class RegistersAccountCommandValidation : AbstractValidator<RegistersAccountCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistersAccountCommandValidation"/> class.
        /// </summary>
        public RegistersAccountCommandValidation()
        {

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be blank")
                .EmailAddress().WithMessage("Invalid email. Please enter in correct format.");
     
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name cannot be blank")
                .Matches(@"^[a-zA-ZÀ-ỹ\s]+$").WithMessage("Full name cannot contain numbers or special characters.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.IdentifyNumber).NotEmpty()
                .WithMessage("Identify Number cannot be blank").Matches(@"^\d{9}$|^\d{12}$")
                .WithMessage("Identify Number must have 9 or 12 digits.");


            RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be left blank.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])")
                .WithMessage("Password must contain uppercase, lowercase, numbers and special characters.");

        }
    }
}
