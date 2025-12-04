using IAM_Service.Application.Interface.IEncryptionService;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IAM_Service.Infrastructure.Security
{
    public class DeterministicAesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _salt;
        private const int IV_SIZE = 16;

        public DeterministicAesEncryptionService(IConfiguration configuration)
        {
            var base64Key = configuration["Encryption:Key"];
            var saltValue = configuration["Encryption:Salt"];

            if (string.IsNullOrEmpty(base64Key))
                throw new InvalidOperationException("Encryption key is missing in configuration.");

            _key = Convert.FromBase64String(base64Key);
            _salt = Encoding.UTF8.GetBytes(saltValue ?? "default-shared-salt");
        }

        /// <summary>
        /// Encrypts deterministically: same plaintext -> same ciphertext.
        /// Prefixes IV to cipherBytes.
        /// </summary>
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            // Deterministic IV
            using var sha = SHA256.Create();
            var ivSource = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText + Convert.ToBase64String(_salt)));
            var iv = ivSource.Take(IV_SIZE).ToArray();

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _key;
            aes.IV = iv;

            byte[] cipherBytes;
            using (var memoryStream = new MemoryStream())
            using (var encryptor = aes.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream, Encoding.UTF8))
            {
                writer.Write(plainText);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                cipherBytes = memoryStream.ToArray();
            }

            // ⭐ Prefix IV + cipherBytes
            var finalBytes = new byte[IV_SIZE + cipherBytes.Length];
            Buffer.BlockCopy(iv, 0, finalBytes, 0, IV_SIZE);
            Buffer.BlockCopy(cipherBytes, 0, finalBytes, IV_SIZE, cipherBytes.Length);

            return Convert.ToBase64String(finalBytes);
        }

        /// <summary>
        /// Decrypts the ciphertext by reading prefixed IV.
        /// </summary>
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] fullCipher;
            try
            {
                fullCipher = Convert.FromBase64String(cipherText);
            }
            catch
            {
                throw new ArgumentException("cipherText is not valid base64.", nameof(cipherText));
            }

            if (fullCipher.Length <= IV_SIZE)
                throw new ArgumentException("cipherText does not contain a valid IV + data.", nameof(cipherText));

            // Extract IV
            var iv = new byte[IV_SIZE];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, IV_SIZE);

            // Extract cipher bytes
            var cipherBytes = new byte[fullCipher.Length - IV_SIZE];
            Buffer.BlockCopy(fullCipher, IV_SIZE, cipherBytes, 0, cipherBytes.Length);

            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _key;
            aes.IV = iv;

            using var memoryStream = new MemoryStream(cipherBytes);
            using var decryptor = aes.CreateDecryptor();
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}
