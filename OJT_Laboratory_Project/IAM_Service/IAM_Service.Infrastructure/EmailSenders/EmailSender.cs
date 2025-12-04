using IAM_Service.Application.Interface.IEmailSender;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace IAM_Service.Infrastructure.EmailSenders
{
    /// <summary>
    /// Implement method from IEmailSender to send email
    /// </summary>
    /// <seealso cref="IAM_Service.Application.Interface.IEmailSender.IEmailSender" />
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// The SMTP server
        /// </summary>
        private readonly string _smtpServer;
        /// <summary>
        /// The SMTP port
        /// </summary>
        private readonly int _smtpPort;
        /// <summary>
        /// The SMTP username
        /// </summary>
        private readonly string _smtpUsername;
        /// <summary>
        /// The SMTP password
        /// </summary>
        private readonly string _smtpPassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailSender"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public EmailSender(IConfiguration configuration)
        {
            _smtpServer = configuration.GetValue<string>("SmtpSettings:SmtpServer", "");
            _smtpPort = configuration.GetValue<int>("SmtpSettings:SmtpPort", 587);
            _smtpUsername = configuration.GetValue<string>("SmtpSettings:SmtpUsername", "");
            _smtpPassword = configuration.GetValue<string>("SmtpSettings:SmtpPassword", "");
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="senderName">Name of the sender.</param>
        /// <param name="senderEmail">The sender email.</param>
        /// <param name="toName">To name.</param>
        /// <param name="toEmail">To email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="textContent">Content of the text.</param>
        public async void SendEmail(string senderName, string senderEmail, string toName, string toEmail,
            string subject, string textContent)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            // Detect HTML content automatically
            var contentType = textContent.Contains("<html>", StringComparison.OrdinalIgnoreCase) ||
                              textContent.Contains("<body>", StringComparison.OrdinalIgnoreCase)
                              ? "html" : "plain";

            message.Body = new TextPart(contentType)
            {
                Text = textContent
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);

                    await client.SendAsync(message);
                    Console.WriteLine("✅ Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Email sending failed: {ex.Message}");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
