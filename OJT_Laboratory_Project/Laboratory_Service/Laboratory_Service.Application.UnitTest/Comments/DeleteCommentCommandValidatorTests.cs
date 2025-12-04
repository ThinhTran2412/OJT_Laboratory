using FluentValidation.TestHelper;
using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.DTOs.Comment;

namespace Laboratory_Service.Application.UnitTest.Comments
{
    public class DeleteCommentCommandValidatorTests
    {
        private readonly DeleteCommentCommandValidator _validator;

        public DeleteCommentCommandValidatorTests()
        {
            _validator = new DeleteCommentCommandValidator();
        }

        [Fact]
        public void Should_Pass_When_Valid_Request()
        {
            var dto = new DeleteCommentDto { CommentId = 5 };

            var result = _validator.TestValidate(new DeleteCommentCommand(dto, "user999"));

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_CommentId_Is_Invalid()
        {
            var dto = new DeleteCommentDto { CommentId = 0 };

            var result = _validator.TestValidate(new DeleteCommentCommand(dto, "user999"));

            result.ShouldHaveValidationErrorFor(c => c.Dto.CommentId);
        }
    }
}
