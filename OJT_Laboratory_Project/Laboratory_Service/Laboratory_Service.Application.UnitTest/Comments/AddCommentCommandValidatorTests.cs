using FluentValidation.TestHelper;
using Laboratory_Service.Application.Comment.Validations;
using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.DTOs.Comment;

namespace Laboratory_Service.Application.UnitTest.Comments
{
    public class AddCommentCommandValidatorTests
    {
        private readonly AddCommentCommandValidator _validator;

        public AddCommentCommandValidatorTests()
        {
            _validator = new AddCommentCommandValidator();
        }

        [Fact]
        public void Should_Pass_When_Valid_Request_With_TestOrder()
        {
            var dto = new CreateCommentDto
            {
                TestOrderId = Guid.NewGuid(),
                TestResultId = new List<int>(),
                Message = "Valid comment"
            };

            var command = new AddCommentCommand(dto, createdBy: "user10");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Message_Is_Empty()
        {
            var dto = new CreateCommentDto
            {
                TestOrderId = Guid.NewGuid(),
                TestResultId = new List<int>(),
                Message = ""
            };

            var command = new AddCommentCommand(dto, createdBy: "user10");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Dto.Message);
        }
    }
}
