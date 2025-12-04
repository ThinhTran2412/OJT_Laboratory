using AutoMapper;
using IAM_Service.Application.Helpers;
using IAM_Service.Application.Interface.IEmailSender;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Application.Interface.IRole;
using IAM_Service.Domain.Entity;
using MediatR;

/// <summary>
/// Handler for processing user creation commands.
/// </summary>
namespace IAM_Service.Application.Users.Command
{
    /// <summary>
    /// Implement IRequestHandler<CreateUserCommand,Unit> to user logic processing
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.Users.Command.CreateUserCommand, MediatR.Unit&gt;" />

    /// Command handler for creating a new user.
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
    {
        /// <summary>
        /// The user repository for accessing user data.
        /// </summary>
        private readonly IUsersRepository _userRepository;
        
        /// <summary>
        /// The role repository for accessing role data.
        /// </summary>
        private readonly IRoleRepository _roleRepository;
        /// <summary>
        ///     The mapper for converting between command and entity.
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor for the CreateUserCommandHandler.
        /// </summary> <param name="userRepository">The user repository for accessing user data.</param>
        /// <param name="mapper">The mapper for converting between command and entity.</param>
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailSender _emailSender;
        public CreateUserCommandHandler(IUsersRepository userRepository, IRoleRepository roleRepository, IMapper mapper, IPasswordHasher passwordHasher, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
         public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists in the system.");
            }

            var checkIndentify = await _userRepository.CheckUserExistsByIdentifyNumberAsync(request.IdentifyNumber);
            if (checkIndentify)
            {
                throw new InvalidOperationException("IndentifyNumder already exists in the system.");
            }


            // Handle role assignment
            int? roleId = request.RoleId;
            if (roleId.HasValue)
            {
                // Validate that the role exists
                var role = await _roleRepository.GetByIdAsync(roleId.Value);
                if (role == null)
                {
                    throw new InvalidOperationException("The specified role does not exist.");
                }
            }
            else
            {
                // If no role specified, find the default Read-Only role
                var defaultRole = await _roleRepository.GetByCodeAsync("READ_ONLY");
                if (defaultRole != null)
                {
                    roleId = defaultRole.RoleId;
                }
                // If no default role exists, we'll create the user without a role
                // and let the business logic handle this case
            }

            var plainPassword = PasswordGenerator.Generate(12);
            var hashedPassword = _passwordHasher.Hash(plainPassword);
            var newUser = _mapper.Map<User>(request);
            newUser.Password = hashedPassword;
            newUser.RoleId = roleId;

            await _userRepository.CreateUser(newUser);

            await Task.Run(() =>
            {
                _emailSender.SendEmail(
                    senderName: "Laboratory Management System",
                    senderEmail: "laboratorymanagement01@gmail.com", 
                    toName: newUser.FullName ?? newUser.Email,
                    toEmail: newUser.Email,
                    subject: "Your new account information",
                    textContent:
                    $"Hello {newUser.FullName ?? newUser.Email},\n\n" +
                    $"Your account has been created successfully.\n" +
                    $"Your temporary password is: {plainPassword}\n\n" +
                    $"Please log in and change your password as soon as possible.\n\n" +
                    $"— Laboratory Management System"
                );
            });

            return Unit.Value;
        }
    }
}
