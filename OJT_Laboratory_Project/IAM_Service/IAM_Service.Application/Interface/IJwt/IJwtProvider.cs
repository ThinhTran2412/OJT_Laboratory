using IAM_Service.Domain.Entity;

namespace IAM_Service.Application.Interface.IJwt
{
    /// <summary>
    /// create methods for interface JwtProvider
    /// </summary>
    public interface IJwtProvider
    {
        /// <summary>
        /// Generates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public Task<string> Generate(User user);
        string GenerateForService(string clientId, string scope);
    }
}
