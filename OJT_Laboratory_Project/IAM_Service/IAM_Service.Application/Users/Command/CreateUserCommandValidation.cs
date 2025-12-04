using FluentValidation;

/// <summary>
/// Validator for the CreateUserCommand to ensure valid input.
/// </summary>
namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// create constraints on properties of user object
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;IAM_Service.Application.Users.Command.CreateUserCommand&gt;" />
    /// Validator for the CreateUserCommand to ensure valid input.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{IAM_Service.Application.Users.Command.CreateUserCommand}" />
    public class CreateUserCommandValidation : AbstractValidator<CreateUserCommand>
    {
        /// <summary>
        /// Constructor that sets up validation rules for the CreateUserCommand.
        /// </summary>
        public CreateUserCommandValidation()
        {   
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be blank")
            
                .EmailAddress().WithMessage("Invalid email. Please enter in correct format.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name cannot be blank")
                .Matches(@"^[a-zA-ZÀ-ỹ\s]+$").WithMessage("Full name cannot contain numbers or special characters.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone Number cannot be blank")
                .Matches("^(\\+?\\d{9,11})$").WithMessage("Invalid phone number. Please enter 9–11 digits.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
             RuleFor(x => x.Age).NotEmpty().WithMessage("Identify Number cannot be blank")
                 .InclusiveBetween(1, 100).WithMessage("Age must be between 1 and 100.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
            RuleFor(x => x.IdentifyNumber).NotEmpty()
                .WithMessage("Identify Number cannot be blank").Matches(@"^\d{9}$|^\d{12}$")
                .WithMessage("Identify Number must have 9 or 12 digits.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
            RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender cannot be blank")
                .Must(g => g.Equals("Male", StringComparison.OrdinalIgnoreCase) || g.Equals("Female", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Gender can only be selected as Male or Female.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            ///  </summary>
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address cannot be blank")
                .MinimumLength(5).WithMessage("Address must be at least 5 characters long.")
                .MaximumLength(255).WithMessage("The address cannot exceed 255 characters.");
            /// <summary>
            /// Constructor that sets up validation rules for the CreateUserCommand.
            /// </summary>
            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Date of birth cannot be left blank.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Date of birth cannot exceed the current date.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.AddYears(-100)))
                .WithMessage("Invalid date of birth (age too old).");

            /// <summary>
            /// Validation rule for RoleId - optional field
            /// </summary>
            RuleFor(x => x.RoleId)
                .Must(id => id > 0)
                .When(x => x.RoleId.HasValue)
                .WithMessage("Role ID must be a positive number when provided.");
        }
    }
}
