using FluentValidation;

namespace Laboratory_Service.Application.Comments.Commands
{
    public class ModifyCommentCommandValidator : AbstractValidator<ModifyCommentCommand>
    {
        public ModifyCommentCommandValidator()
        {
            RuleFor(x => x.CommentId)
                .NotEmpty().WithMessage("CommentId is required.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Content cannot be empty.")
                .MinimumLength(3).WithMessage("Content must be at least 3 characters.")
                .MaximumLength(2000).WithMessage("Content cannot exceed 2000 characters.");
        }
    }
}
