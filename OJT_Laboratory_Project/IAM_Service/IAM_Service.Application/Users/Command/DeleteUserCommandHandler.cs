using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Command.DeleteUserCommand, MediatR.Unit&gt;" />
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public DeleteUserCommandHandler(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">User not found.</exception>
        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            await _userRepository.DeleteAsync(user);

            return Unit.Value;
        }
    }
}
