using IAM_Service.Application.Common.Exceptions;
using IAM_Service.Application.Common.Security;
using IAM_Service.Application.Interface.IJwt;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IUser;
using MediatR;
using Microsoft.Extensions.Options;

namespace IAM_Service.Application.Login
{
    /// <summary>
    /// Handles the login command by validating user credentials and generating a JWT token.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
        /// </summary>
        private readonly IUsersRepository _userRepository;
        /// <summary>
        /// The JWT provider for token generation.
        /// </summary>
        private readonly IJwtProvider _jwtProvider;
        /// <summary>
        /// The password hasher for verifying passwords.
        /// </summary>
        private readonly IPasswordHasher _passwordHasher;
        /// <summary> The lockout options for handling account lockouts. </summary>
        private readonly LockoutOptions _lockout;
        private readonly IRefreshTokenProvider _refreshTokenProvider;

        /// <summary>
        /// Constructor for the LoginCommandHandler.
        /// </summary> <param name="userRepository">The user repository for accessing user data.</param>
        /// <param name="jwtProvider">The JWT provider for token generation.</param>
        /// <param name="passwordHasher">The password hasher for verifying passwords.</param>
        /// <param name="lockoutOptions">The lockout options for handling account lockouts.</param>
        public LoginCommandHandler(IUsersRepository userRepository, IJwtProvider jwtProvider, IPasswordHasher passwordHasher, IOptions<LockoutOptions> lockoutOptions, IRefreshTokenProvider refreshTokenProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _lockout = lockoutOptions.Value;
            _refreshTokenProvider = refreshTokenProvider;
        }
        /// <summary>
        /// Handles the login command by validating user credentials and generating a JWT token.
        /// </summary>  <param name="request">The login command containing user credentials.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A JWT token if the login is successful.</returns>
        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new InvalidCredentialsException("Invalid credentials.", _lockout.MaxFailedAccessAttempts);
            }

            if (_lockout.Enabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                var timeRemaining = (int)(user.LockoutEnd.Value - DateTime.UtcNow).TotalSeconds;

                // **THROW ACCOUNT LOCKED EXCEPTION**
                throw new AccountLockedException(
                    $"Account locked until {user.LockoutEnd.Value:yyyy-MM-dd HH:mm:ss} UTC.",
                    timeRemaining
                );
            }


            bool isValidPassword = _passwordHasher.Verify(request.Password, user.Password);
            if (!isValidPassword)
            {
                if (_lockout.Enabled)
                {
                    user.FailedLoginAttempts += 1;
                    user.LastFailedLoginAt = DateTime.UtcNow;

                    if (user.FailedLoginAttempts >= _lockout.MaxFailedAccessAttempts)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(_lockout.LockoutMinutes);
                        user.FailedLoginAttempts = 0;
                        await _userRepository.UpdateAsync(user);
                        await _userRepository.SaveChangesAsync();

                        var timeRemaining = (int)_lockout.LockoutMinutes * 60;
                        throw new AccountLockedException(
                           $"You've entered incorrect credentials too many times. Your account is locked for {_lockout.LockoutMinutes} minutes.",
                           timeRemaining
                        );
                    }

                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();

                    int attemptsRemaining = _lockout.MaxFailedAccessAttempts - user.FailedLoginAttempts;
                    throw new InvalidCredentialsException(
                         $"Invalid credentials. You have {attemptsRemaining} attempts remaining.",
                         attemptsRemaining
                    );
                }
                throw new InvalidCredentialsException("Invalid credentials.", 0);
            }

            //If Login successful, reset failed attempts and lockout
            if (_lockout.Enabled && (user.FailedLoginAttempts > 0 || user.LockoutEnd.HasValue))
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();
            }
            // Generate access token
            string accessToken = await _jwtProvider.Generate(user);

            // Generate refresh token
            var refreshToken = await _refreshTokenProvider.GenerateAsync(user);

            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
            };
        }
    }
}
