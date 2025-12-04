using FluentValidation;
using Laboratory_Service.Application.Comments.Commands;

namespace Laboratory_Service.Application.Comment.Validations
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.Comments.Commands.AddCommentCommand&gt;" />
    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommentCommandValidator"/> class.
        /// </summary>
        public AddCommentCommandValidator()
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .WithMessage("Comment data is required.");

            RuleFor(x => x.Dto.Message)
                .NotEmpty()
                .WithMessage("Comment message cannot be empty.");
        }
    }
}
