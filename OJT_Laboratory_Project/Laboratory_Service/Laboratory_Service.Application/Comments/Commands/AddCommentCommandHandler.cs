using AutoMapper;
using Laboratory_Service.Application.Interface;
using MediatR;

namespace Laboratory_Service.Application.Comments.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;Laboratory_Service.Application.Comments.Commands.AddCommentCommand, System.Int64&gt;" />
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, int>
    {
        /// <summary>
        /// The comment repo
        /// </summary>
        private readonly ICommentRepository _commentRepo;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommentCommandHandler"/> class.
        /// </summary>
        /// <param name="commentRepo">The comment repo.</param>
        /// <param name="mapper">The mapper.</param>
        public AddCommentCommandHandler(ICommentRepository commentRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<int> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Entity.Comment>(request.Dto);

            entity.CreatedBy = request.CreatedBy;       // set từ JWT
            entity.CreatedDate = DateTime.UtcNow;

            await _commentRepo.AddAsync(entity);
            await _commentRepo.SaveChangesAsync();

            return entity.CommentId;
        }
    }
}
