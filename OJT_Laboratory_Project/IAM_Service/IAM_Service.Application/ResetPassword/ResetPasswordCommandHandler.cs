using IAM_Service.Application.Common.Exceptions;
using IAM_Service.Application.Interface.IPasswordHasher;
using IAM_Service.Application.Interface.IPasswordResetRepository;
using IAM_Service.Application.Interface.IUser;
using MediatR;

namespace IAM_Service.Application.ResetPassword
{
    /// <summary>
    /// create ResetPasswordCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.ResetPassword.ResetPasswordCommand, MediatR.Unit&gt;" />
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository _userRepository;
        /// <summary>
        /// The password hasher
        /// </summary>
        private readonly IPasswordHasher _passwordHasher;
        /// <summary>
        /// The password reset repository
        /// </summary>
        private readonly IPasswordResetRepository _passwordResetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="passwordHasher">The password hasher.</param>
        /// <param name="passwordResetRepository">The password reset repository.</param>
        public ResetPasswordCommandHandler(
            IUsersRepository userRepository,
            IPasswordHasher passwordHasher,
            IPasswordResetRepository passwordResetRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _passwordResetRepository = passwordResetRepository;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        /// <exception cref="IAM_Service.Application.Common.Exceptions.InvalidOrExpiredTokenException">Invalid or expired token</exception>
        /// <exception cref="IAM_Service.Application.Common.Exceptions.NotFoundException">User not found</exception>
        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var resetRecord = await _passwordResetRepository.GetByTokenAsync(request.Token);
            if (resetRecord == null || resetRecord.ExpiresAt < DateTime.UtcNow || resetRecord.IsUsed)
                throw new InvalidOrExpiredTokenException("Invalid or expired token");

            var user = await _userRepository.GetByIdAsync(resetRecord.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            user.Password = _passwordHasher.Hash(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            await _passwordResetRepository.MarkUsedAsync(resetRecord);

            return Unit.Value;
        }
    }
}
