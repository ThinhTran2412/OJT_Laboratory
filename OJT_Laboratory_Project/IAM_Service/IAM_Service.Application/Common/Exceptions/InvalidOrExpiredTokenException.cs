namespace IAM_Service.Application.Common.Exceptions
{
    /// <summary>
    /// Create exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidOrExpiredTokenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidOrExpiredTokenException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidOrExpiredTokenException(string message) : base(message) { }
    }
}
