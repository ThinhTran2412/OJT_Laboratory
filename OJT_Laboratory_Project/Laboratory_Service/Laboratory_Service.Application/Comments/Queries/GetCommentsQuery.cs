using Laboratory_Service.Application.DTOs.Comment;
using MediatR;

namespace Laboratory_Service.Application.Comments.Queries
{
    public class GetCommentsQuery : IRequest<List<CommentDto>>
    {
        public List<int> TestResultId { get; set; }

        public GetCommentsQuery(List<int> testResultId)
        {
            TestResultId = testResultId;
        }
    }
}
