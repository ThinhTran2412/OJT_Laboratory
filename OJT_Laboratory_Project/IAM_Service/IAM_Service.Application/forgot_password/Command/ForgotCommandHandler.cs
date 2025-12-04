using IAM_Service.Application.Common.Exceptions;
using IAM_Service.Application.Interface.IEmailSender;
using IAM_Service.Application.Interface.IPasswordResetRepository;
using IAM_Service.Application.Interface.IUser;
using IAM_Service.Domain.Entity;
using MediatR;
using System.Security.Cryptography;

namespace IAM_Service.Application.forgot_password.Command
{
    /// <summary>
    /// Create ForgotCommandHandler
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler&lt;IAM_Service.Application.forgot_password.Command.ForgotCommand&gt;" />
    public class ForgotCommandHandler : IRequestHandler<ForgotCommand>
    {
        /// <summary>
        /// The users repository
        /// </summary>
        private readonly IUsersRepository _usersRepository;
        /// <summary>
        /// The password reset repository
        /// </summary>
        private readonly IPasswordResetRepository _passwordResetRepository;
        /// <summary>
        /// The email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotCommandHandler"/> class.
        /// </summary>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="passwordResetRepository">The password reset repository.</param>
        /// <param name="emailSender">The email sender.</param>
        public ForgotCommandHandler(
            IUsersRepository usersRepository,
            IPasswordResetRepository passwordResetRepository,
            IEmailSender emailSender)
        {
            _usersRepository = usersRepository;
            _passwordResetRepository = passwordResetRepository;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="System.Exception">Email not exist, please try again!</exception>
        public async Task Handle(ForgotCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetByEmailAsync(request.EmailForgot);
            if (user == null)
                throw new NotFoundException("Email not exist, please try again!");

            // Generate secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiresAt = DateTime.UtcNow.AddMinutes(15);

            var reset = new PasswordReset
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = expiresAt,
                IsUsed = false
            };

            await _passwordResetRepository.AddAsync(reset);
            await _passwordResetRepository.SaveChangesAsync();

            // Create reset link
            var resetLink = $"http://localhost:5173/reset-password?token={Uri.EscapeDataString(token)}";

            // Email subject
            var subject = "🔐 Password Reset Request - Laboratory";

            // Email body (HTML)
            var body = $@"
<html>
  <body style='font-family: Arial, sans-serif; background-color: #f9fafb; padding: 20px; color: #333;'>
    <table align='center' width='100%' style='max-width: 600px; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1); padding: 30px;'>
      <tr>
        <td style='text-align: center;'>
          <h2 style='color: #2563eb;'>IAM Service Support</h2>
          <hr style='border: none; height: 1px; background-color: #e5e7eb; margin: 20px 0;'/>
        </td>
      </tr>
      <tr>
        <td>
          <p style='font-size: 16px;'>Hello <strong>{user.FullName}</strong>,</p>
          <p style='font-size: 15px; line-height: 1.6;'>
            You requested to reset your password. Click the button below to continue:
          </p>

          <p style='text-align: center; margin: 30px 0;'>
            <a href='{resetLink}' 
               style='background-color: #2563eb; color: white; text-decoration: none; padding: 12px 24px; border-radius: 6px; font-weight: bold;'>
              Reset Password
            </a>
          </p>

          <p style='font-size: 14px; color: #555;'>
            ⚠️ This link will expire in <strong>15 minutes</strong> for your security.
          </p>

          <p style='font-size: 14px; color: #777;'>
            If you did not request this action, please ignore this email.
          </p>

          <hr style='border: none; height: 1px; background-color: #e5e7eb; margin: 30px 0;'/>
          <p style='font-size: 12px; text-align: center; color: #999;'>
            © {DateTime.UtcNow.Year} IAM Service. All rights reserved.<br/>
            This is an automated message — please do not reply.
          </p>
        </td>
      </tr>
    </table>
  </body>
</html>
";

            // Send email
            _emailSender.SendEmail(
                senderName: "Service Support",
                senderEmail: "no-reply@iam-service.com",
                toName: user.FullName,
                toEmail: user.Email,
                subject: subject,
                textContent: body
            );
        }
    }
}
