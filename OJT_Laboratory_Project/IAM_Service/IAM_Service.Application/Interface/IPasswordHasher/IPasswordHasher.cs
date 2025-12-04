namespace IAM_Service.Application.Interface.IPasswordHasher
{
    /// <summary>
    /// create methods for interface PasswordHasher
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        string Hash(string password);
        /// <summary>
        /// Verifies the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns></returns>
        bool Verify(string password, string passwordHash);
    }
}
