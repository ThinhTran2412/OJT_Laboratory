using FluentValidation;

namespace Laboratory_Service.Application.Patients.Commands
{
    /// <summary>
    /// create validation for CreatePatientByIdentityCommandValidator
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.Patients.Commands.CreatePatientByIdentityCommand&gt;" />
    public class CreatePatientByIdentityCommandValidator : AbstractValidator<CreatePatientByIdentityCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePatientByIdentityCommandValidator"/> class.
        /// </summary>
        public CreatePatientByIdentityCommandValidator()
        {
            RuleFor(x => x.IdentifyNumber)
                .NotEmpty().WithMessage("Identification number is required")
                .Length(9, 100).WithMessage("Identification number must be between 9 and 20 characters");
        }
    }
}
