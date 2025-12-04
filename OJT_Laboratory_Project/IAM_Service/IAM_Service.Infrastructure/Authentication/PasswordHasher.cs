using IAM_Service.Application.Interface.IPasswordHasher;
using System.Security.Cryptography;

/// <summary>
/// Implementation of password hashing and verification using PBKDF2 with SHA-512.
/// </summary>
namespace IAM_Service.Infrastructure.Authentication
{
    /// <summary>
    /// Provides methods to hash and verify passwords securely.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        // Configuration constants for hashing
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        // Constants for hashing
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        /// <summary>
        /// Hashes a plain text password using PBKDF2 with SHA-512.
        /// </summary>  <param name="password">The plain text password to hash.</param>
        /// <returns>A hashed password in the format "hash-salt".</returns>
        ///     
        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            // Combine hash and salt into a single string for storage
            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        /// <summary>
        /// Verifies a plain text password against a stored hashed password.
        /// </summary>  <param name="password">The plain text password to verify.</param>
        /// <param name="passwordHash">The stored hashed password in the format "hash-salt".</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        public bool Verify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            // Recompute the hash using the same salt and parameters
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
