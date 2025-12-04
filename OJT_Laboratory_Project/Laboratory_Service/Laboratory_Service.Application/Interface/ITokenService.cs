namespace Laboratory_Service.Application.Interface
{
    /// <summary>
    /// Create Method for interface TokenService
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gets the access token asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<string?> GetAccessTokenAsync();
    }
}
