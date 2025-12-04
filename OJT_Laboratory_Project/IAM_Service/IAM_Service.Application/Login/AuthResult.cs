namespace IAM_Service.Application.Login
{
    /// <summary>
    /// create attribute for class AuthResult
    /// </summary>
    public class AuthResult
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
