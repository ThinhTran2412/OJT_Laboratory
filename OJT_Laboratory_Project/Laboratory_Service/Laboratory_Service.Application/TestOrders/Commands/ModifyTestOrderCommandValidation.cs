using FluentValidation;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Create class validation for ModifyTestOrderCommand
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.TestOrders.Commands.ModifyTestOrderCommand&gt;" />
    public class ModifyTestOrderCommandValidator : AbstractValidator<ModifyTestOrderCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyTestOrderCommandValidator"/> class.
        /// </summary>
        public ModifyTestOrderCommandValidator()
        {
            RuleFor(x => x.TestOrderId)
                .NotEmpty()
                .WithMessage("TestOrderId is required.");

            RuleFor(x => x.IdentifyNumber)
                .NotEmpty().WithMessage("IdentifyNumber is required.")
                .Matches(@"^\d{9}$|^\d{12}$")
                .WithMessage("IdentifyNumber must contain 9 or 12 digits.");

            RuleFor(x => x.Priority)
                .Must(p => p == null || new[] { "Normal", "Urgent", "Emergency" }.Contains(p))
                .WithMessage("Priority must be one of: Normal, Urgent, Emergency.");


            RuleFor(x => x.Note)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters.");

        }
    }
}
