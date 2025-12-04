namespace IAM_Service.Application.Interface.IEncryptionService
{
    /// <summary>
    /// create methods for interface EncryptionService
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        string Encrypt(string plainText);
        /// <summary>
        /// Decrypts the specified cipher text.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns></returns>
        string Decrypt(string cipherText);
    }
}
