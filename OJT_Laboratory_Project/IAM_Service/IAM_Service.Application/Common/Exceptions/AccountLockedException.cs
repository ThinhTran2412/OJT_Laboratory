namespace IAM_Service.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a user account is locked due to multiple failed login attempts.
    /// </summary>
    public class AccountLockedException : Exception
    {
        /// <summary> Gets the time left in seconds until the account is unlocked. </summary>
        public int TimeLeftSeconds { get; }

        // THUỘC TÍNH ĐƯỢC THÊM để tương thích với Unit Test (sử dụng tên cũ là TimeRemainingInSeconds)
        /// <summary> Gets the time left in seconds until the account is unlocked (Alias for compatibility with older tests). </summary>
        public int TimeRemainingInSeconds => TimeLeftSeconds;

        /// <summary> Initializes a new instance of the <see cref="AccountLockedException"/> class with a specified error message and time left. </summary>
        /// <param name="message">The message that describes the error.</param>
        public AccountLockedException(string message, int timeLeftSeconds)
            : base(message)
        {
            TimeLeftSeconds = timeLeftSeconds;
        }
    }
}