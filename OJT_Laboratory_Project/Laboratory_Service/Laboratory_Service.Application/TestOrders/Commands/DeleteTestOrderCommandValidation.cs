using FluentValidation;

namespace Laboratory_Service.Application.TestOrders.Commands
{
    /// <summary>
    /// Create class validation for  DeleteTestOrderCommand
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.TestOrders.Commands.DeleteTestOrderCommand&gt;" />
    public class DeleteTestOrderCommandValidation : AbstractValidator<DeleteTestOrderCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTestOrderCommandValidation"/> class.
        /// </summary>
        public DeleteTestOrderCommandValidation()
        {
            RuleFor(x => x.TestOrderId)
                .NotEmpty().WithMessage("TestOrderId is required.");

            RuleFor(x => x.DeletedBy)
                .NotEmpty().WithMessage("DeletedBy is required.");
        }
    }
}
