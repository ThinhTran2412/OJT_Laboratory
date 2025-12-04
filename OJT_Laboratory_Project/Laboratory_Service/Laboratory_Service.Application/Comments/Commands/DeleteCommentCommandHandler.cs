using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.Comments.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Comments.Commands.DeleteCommentCommand, System.Boolean&gt;" />
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        /// <summary>
        /// The comment repo
        /// </summary>
        private readonly ICommentRepository _commentRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommentCommandHandler"/> class.
        /// </summary>
        /// <param name="commentRepo">The comment repo.</param>
        public DeleteCommentCommandHandler(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var entity = await _commentRepo.GetByIdAsync(request.Dto.CommentId);

            // Nếu không tìm thấy hoặc đã bị xóa → return false
            if (entity is null || entity.IsDeleted)
                return false;

            // Thực hiện soft delete
            entity.IsDeleted = true;
            entity.DeletedBy = request.DeletedBy;   // <-- lấy từ JWT
            entity.DeletedDate = DateTime.UtcNow;

            await _commentRepo.SaveChangesAsync();

            return true;
        }
    }
}
