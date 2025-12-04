using Laboratory_Service.Application.DTOs.Comment;
using MediatR;
using System.Text.Json.Serialization;

namespace Laboratory_Service.Application.Comments.Commands
{
    public class ModifyCommentCommand : IRequest<CommentDto>
    {
        [JsonIgnore]
        public int CommentId { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore]
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
