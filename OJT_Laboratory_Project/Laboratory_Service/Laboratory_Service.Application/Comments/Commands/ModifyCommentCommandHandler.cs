using AutoMapper;
using Laboratory_Service.Application.DTOs.Comment;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.Comments.Commands
{
    public class ModifyCommentCommandHandler : IRequestHandler<ModifyCommentCommand, CommentDto>
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public ModifyCommentCommandHandler(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CommentDto> Handle(ModifyCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _repository.GetByIdAsync(request.CommentId);

            if (comment == null)
                throw new Exception("Comment not found!");

            comment.Message = request.Message;
            comment.ModifiedDate = DateTime.UtcNow;
            comment.ModifiedBy = request.ModifiedBy;

            await _repository.UpdateAsync(comment);

            return _mapper.Map<CommentDto>(comment);
        }
    }
}
