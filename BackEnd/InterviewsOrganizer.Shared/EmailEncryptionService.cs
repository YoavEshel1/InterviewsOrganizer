using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace InterviewsOrganizer.Shared
{
    /// <summary>
    /// AES-256 encryption (via ASP.NET Core Data Protection) + deterministic HMAC-SHA256 blind-index
    /// hashing for email addresses. The HMAC secret key is read from configuration
    /// ("EmailEncryption:HmacSecretKey") and must be identical across every app sharing the database
    /// (API and the EmailHasher CLI), otherwise computed hashes will not match existing rows.
    /// </summary>
    public class EmailEncryptionService : IEmailEncryptionService
    {
        // Purpose string scopes the protector so this payload can't be swapped with a differently-purposed one.
        private const string ProtectorPurpose = "InterviewsOrganizer.Email.v1";

        private readonly IDataProtector _protector;
        private readonly byte[] _hmacKey;

        public EmailEncryptionService(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration)
        {
            _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);

            var secretKey = configuration["EmailEncryption:HmacSecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException(
                    "Configuration value \"EmailEncryption:HmacSecretKey\" is required and must be identical across all apps sharing the database.");
            }

            _hmacKey = Encoding.UTF8.GetBytes(secretKey);
        }

        public string Encrypt(string plainEmail)
        {
            if (string.IsNullOrEmpty(plainEmail)) return plainEmail;
            return _protector.Protect(plainEmail);
        }

        public string Decrypt(string cipherEmail)
        {
            if (string.IsNullOrEmpty(cipherEmail)) return cipherEmail;
            return _protector.Unprotect(cipherEmail);
        }

        public string ComputeEmailHash(string email)
        {
            if (string.IsNullOrEmpty(email)) return string.Empty;

            var normalized = NormalizeEmail(email);
            var bytes = Encoding.UTF8.GetBytes(normalized);
            var hash = HMACSHA256.HashData(_hmacKey, bytes);
            return Convert.ToHexString(hash); // uppercase hex, stable and index-friendly
        }

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
