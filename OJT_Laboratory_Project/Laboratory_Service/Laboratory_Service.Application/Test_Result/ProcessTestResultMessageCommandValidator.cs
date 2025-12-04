using FluentValidation;

namespace Laboratory_Service.Application.Test_Result
{
    /// <summary>
    /// Creates the validator for <see cref="ProcessTestResultMessageCommand"/>.
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.Test_Result.ProcessTestResultMessageCommand&gt;" />
    public class ProcessTestResultMessageCommandValidator : AbstractValidator<ProcessTestResultMessageCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessTestResultMessageCommandValidator"/> class.
        /// </summary>
        public ProcessTestResultMessageCommandValidator()
        {
            // Validate TestOrderId
            RuleFor(x => x.TestOrderId)
                .NotEmpty()
                .WithMessage("TestOrderId is required.");
        }
    }
}
