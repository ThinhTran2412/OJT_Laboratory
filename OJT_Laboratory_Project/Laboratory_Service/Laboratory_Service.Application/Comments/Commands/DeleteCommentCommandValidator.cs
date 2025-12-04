using FluentValidation;

namespace Laboratory_Service.Application.Comments.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator&lt;Laboratory_Service.Application.Comments.Commands.DeleteCommentCommand&gt;" />
    public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommentCommandValidator"/> class.
        /// </summary>
        public DeleteCommentCommandValidator()
        {
            RuleFor(x => x.Dto.CommentId)
                .GreaterThan(0)
                .WithMessage("CommentId must be greater than zero.");
        }
    }
}
