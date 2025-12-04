using Laboratory_Service.Application.DTOs.Comment;
using MediatR;

namespace Laboratory_Service.Application.Comments.Queries
{
    public class GetCommentsByTestOrderIdQuery : IRequest<List<CommentDto>>
    {
        public Guid TestOrderId { get; set; }

        public GetCommentsByTestOrderIdQuery(Guid testOrderId)
        {
            TestOrderId = testOrderId;
        }
    }
}

