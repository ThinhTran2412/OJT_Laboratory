using AutoMapper;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.Comments.Queries
{
    public class GetCommentsByTestOrderIdQueryHandler : IRequestHandler<GetCommentsByTestOrderIdQuery, List<CommentDto>>
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public GetCommentsByTestOrderIdQueryHandler(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CommentDto>> Handle(GetCommentsByTestOrderIdQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repository.GetByTestOrderIdAsync(request.TestOrderId);
            return _mapper.Map<List<CommentDto>>(comments);
        }
    }
}

