namespace IAM_Service.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when user credentials are invalid.
    /// </summary>
    public class InvalidCredentialsException : Exception
    {
        /// <summary> Gets the number of remaining login attempts before lockout. </summary>
        public int AttemptsRemaining { get; }

        /// <summary> Initializes a new instance of the <see cref="InvalidCredentialsException"/> class with a specified error message and remaining attempts. </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="attemptsRemaining">The number of remaining login attempts before lockout.</param>
        public InvalidCredentialsException(string message, int attemptsRemaining)
            : base(message)
        {
            AttemptsRemaining = attemptsRemaining;
        }
    }
}