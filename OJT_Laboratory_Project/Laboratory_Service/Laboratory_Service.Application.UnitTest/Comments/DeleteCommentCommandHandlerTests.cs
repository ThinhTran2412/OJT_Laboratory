using Laboratory_Service.Application.Comments.Commands;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.Interface;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Laboratory_Service.Application.UnitTest.Comments
{
    public class DeleteCommentCommandHandlerTests
    {
        private readonly Mock<ICommentRepository> _commentRepoMock;

        public DeleteCommentCommandHandlerTests()
        {
            _commentRepoMock = new Mock<ICommentRepository>();
        }

        [Fact]
        public async Task Handle_ShouldSoftDeleteComment_WhenCommentExists()
        {
            // Arrange
            var existingComment = new Domain.Entity.Comment
            {
                CommentId = 1,
                Message = "Testing delete",
                IsDeleted = false
            };

            _commentRepoMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingComment);

            var dto = new DeleteCommentDto { CommentId = 1 };

            string jwtUserId = "user999"; // mock user from JWT
            var command = new DeleteCommentCommand(dto, jwtUserId);

            var handler = new DeleteCommentCommandHandler(_commentRepoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.True(existingComment.IsDeleted);
            Assert.Equal(jwtUserId, existingComment.DeletedBy);

            _commentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommentDoesNotExist()
        {
            // Arrange
            _commentRepoMock
                .Setup(x => x.GetByIdAsync(100))
                .ReturnsAsync((Domain.Entity.Comment?)null);

            var dto = new DeleteCommentDto { CommentId = 100 };
            var command = new DeleteCommentCommand(dto, "user777");

            var handler = new DeleteCommentCommandHandler(_commentRepoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _commentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommentAlreadyDeleted()
        {
            // Arrange
            var deletedComment = new Domain.Entity.Comment
            {
                CommentId = 2,
                IsDeleted = true
            };

            _commentRepoMock
                .Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(deletedComment);

            var dto = new DeleteCommentDto { CommentId = 2 };
            var command = new DeleteCommentCommand(dto, "user555");

            var handler = new DeleteCommentCommandHandler(_commentRepoMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _commentRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
