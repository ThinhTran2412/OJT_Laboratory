namespace IAM_Service.Application.Interface.IEmailSender
{
    /// <summary>
    /// create methods for interface EmailSender
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="senderName">Name of the sender.</param>
        /// <param name="senderEmail">The sender email.</param>
        /// <param name="toName">To name.</param>
        /// <param name="toEmail">To email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="textContent">Content of the text.</param>
        void SendEmail(string senderName, string senderEmail, string toName, string toEmail,
            string subject, string textContent);
    }
}
