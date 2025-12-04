using AutoMapper;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Domain.Entity;
using MediatR;

namespace IAM_Service.Application.Registers.Command
{
    /// <summary>
    /// Handle Registers Account for user
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Registers.Command.RegistersAccountCommand, MediatR.Unit&gt;" />
    public class RegistersAccountCommandHandler : IRequestHandler<RegistersAccountCommand, Unit>
    {
        /// <summary>
        /// The users repository
        /// </summary>
        private readonly IUsersRepository _usersRepository;
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// The password hasher
        /// </summary>
        private readonly IPasswordHasher _passwordHasher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistersAccountCommandHandler"/> class.
        /// </summary>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="passwordHasher">The password hasher.</param>
        public RegistersAccountCommandHandler(IUsersRepository usersRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Email already exists in the system.</exception>
        public async Task<Unit> Handle(RegistersAccountCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _usersRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists in the system.");
            }
            var passwordHash = _passwordHasher.Hash(request.Password);
            var newUser = _mapper.Map<User>(request);
            newUser.Password = passwordHash;
            newUser.RoleId = 5;
            await _usersRepository.CreateUser(newUser);
            return Unit.Value;
        }
    }
}
