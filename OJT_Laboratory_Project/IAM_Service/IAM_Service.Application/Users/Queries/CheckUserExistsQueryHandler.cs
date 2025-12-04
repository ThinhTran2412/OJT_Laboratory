using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.Users.Queries
{
    /// <summary>
    /// Handler for the CheckUserExistsQuery.
    /// This is the Use Case/Interactor in the Application Layer responsible for the business logic 
    /// of verifying user existence by Identify Number.
    /// </summary>
    public class CheckUserExistsQueryHandler : IRequestHandler<CheckUserExistsQuery, bool>
    {

        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckUserExistsQueryHandler"/> class.
        /// </summary>
        /// <param name="usersRepository">The repository interface implementation from the Infrastructure Layer.</param>
        public CheckUserExistsQueryHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Handles the process of checking if a user exists by their Identify Number.
        /// </summary>
        /// <param name="request">The incoming CheckUserExistsQuery request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        public async Task<bool> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
        {
            // Application Business Logic: 
            // In this simple case, the logic directly delegates the call to the repository.
            // If complex validation, caching, or domain services were involved, they would be executed here.

            return await _usersRepository.CheckUserExistsByIdentifyNumberAsync(request.IdentifyNumber);
        }
    }
}