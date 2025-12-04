using AutoMapper;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.Comments.Queries
{
    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<CommentDto>>
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public GetCommentsQueryHandler(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repository.GetByTestResultIdAsync(request.TestResultId);

            comments = comments
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return _mapper.Map<List<CommentDto>>(comments);
        }
    }
}
