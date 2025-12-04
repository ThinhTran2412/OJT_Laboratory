using Laboratory_Service.Application.DTOs.Comment;
using MediatR;
using System.Text.Json.Serialization;

namespace Laboratory_Service.Application.Comments.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequest&lt;System.Int64&gt;" />
    public class AddCommentCommand(CreateCommentDto dto, string createdBy) : IRequest<int>
    {
        /// <summary>
        /// Gets the dto.
        /// </summary>
        /// <value>
        /// The dto.
        /// </value>
        public CreateCommentDto Dto { get; set; } = dto;
        /// <summary>
        /// Gets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        [JsonIgnore]
        public string CreatedBy { get; } = createdBy;
    }
}
