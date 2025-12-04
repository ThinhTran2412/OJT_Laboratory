namespace IAM_Service.Infrastructure.Authentication
{
    /// <summary>
    /// Configuration options for JWT authentication.
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// The issuer of the JWT.
        /// </summary>
        public string? Issuer { get; init; }
        /// <summary> The audience for the JWT.
        /// </summary>
        public string? Audience { get; init; }
        /// <summary> The secret key used to sign the JWT.
        /// </summary>
        public string? SecretKey { get; init; }
        /// <summary> The expiration time in minutes for the JWT.
        public int ExpirationMinutes { get; init; }
    }
}
