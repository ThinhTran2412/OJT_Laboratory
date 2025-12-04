using Laboratory_Service.Application.DTOs.Comment;
using MediatR;

namespace Laboratory_Service.Application.Comments.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Boolean&gt;" />
    public class DeleteCommentCommand(DeleteCommentDto dto, string deleteBy) : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the dto.
        /// </summary>
        /// <value>
        /// The dto.
        /// </value>
        public DeleteCommentDto Dto { get; set; } = dto;
        /// <summary>
        /// Gets the deleted by.
        /// </summary>
        /// <value>
        /// The deleted by.
        /// </value>
        public string DeletedBy { get; } = deleteBy;
    }
}
