using FluentValidation;

namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// FluentValidation rules for validating update user request.
    /// Fields are optional — validate only when values are provided.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;IAM_Service.Application.Users.Command.UpdateUserCommand&gt;" />
    /// <seealso cref="FluentValidation.AbstractValidator&lt;IAM_Service.Application.Users.Command.UpdateUserCommand&gt;" />
    public class UpdateUserCommandValidation : AbstractValidator<UpdateUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserCommandValidation" /> class.
        /// </summary>
        public UpdateUserCommandValidation()
        {
            /// <summary>Validate UserId — must exist</summary>
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is required and must be greater than 0.");

            /// <summary>Validate Full name if provided</summary>
            RuleFor(x => x.FullName)
                .Matches(@"^[a-zA-ZÀ-ỹ\s]+$").WithMessage("Full name cannot contain numbers or special characters.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));
            /// <summary>Validate Email if provided</summary>
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email. Please enter in correct format.");

            /// <summary>Validate Phone number if provided</summary>
            RuleFor(x => x.PhoneNumber)
                .Matches("^(\\+?\\d{9,11})$").WithMessage("Invalid phone number. Please enter 9–11 digits.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            /// <summary>Validate Age if provided</summary>
            RuleFor(x => x.Age)
                .InclusiveBetween(1, 100).WithMessage("Age must be between 1 and 100.");

            /// <summary>Validate Gender if provided</summary>
            RuleFor(x => x.Gender)
                .Must(g => g.Equals("Male", StringComparison.OrdinalIgnoreCase)
                        || g.Equals("Female", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Gender can only be Male or Female.")
                .When(x => !string.IsNullOrWhiteSpace(x.Gender));

            /// <summary>Validate Address if provided</summary>
            RuleFor(x => x.Address)
                .MinimumLength(5).WithMessage("Address must be at least 5 characters.")
                .MaximumLength(255).WithMessage("Address cannot exceed 255 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            /// <summary>Validate DateOfBirth if not default</summary>
            RuleFor(x => x.DateOfBirth)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Date of birth cannot exceed the current date.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now.AddYears(-100)))
                .WithMessage("Invalid date of birth.")
                .When(x => x.DateOfBirth != default);

            /// <summary>Validate PrivilegeIds if provided</summary>
            RuleForEach(x => x.PrivilegeIds)
                .GreaterThan(0).WithMessage("PrivilegeId must be greater than 0.")
                .When(x => x.PrivilegeIds != null && x.PrivilegeIds.Any());

        }
    }
}
