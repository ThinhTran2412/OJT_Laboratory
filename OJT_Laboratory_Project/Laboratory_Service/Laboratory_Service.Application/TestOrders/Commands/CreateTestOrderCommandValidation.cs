using FluentValidation;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Validation data for CreateTestOrderCommand
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.TestOrders.Commands.CreateTestOrderCommand&gt;" />
    public class CreateTestOrderCommandValidator : AbstractValidator<CreateTestOrderCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTestOrderCommandValidator"/> class.
        /// </summary>
        public CreateTestOrderCommandValidator()
        {
            RuleFor(x => x.IdentifyNumber)
                .NotEmpty().WithMessage("IdentifyNumber is required.")
                .Matches(@"^\d{9}$|^\d{12}$")
                .WithMessage("IdentifyNumber must contain either 9 or 12 digits only.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.")
                .Must(p => new[] { "Normal", "Urgent", "Emergency" }.Contains(p))
                .WithMessage("Priority must be one of: Normal, Urgent, Emergency.");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("Note cannot exceed 500 characters.");
        }
    }
}
