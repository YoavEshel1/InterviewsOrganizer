namespace InterviewsOrganizer.Shared
{
    /// <summary>
    /// Provides AES-256 encryption/decryption of email addresses (via ASP.NET Core Data Protection)
    /// and a deterministic HMAC-SHA256 "blind index" hash used for equality lookups without
    /// ever storing or querying plaintext email addresses.
    /// </summary>
    public interface IEmailEncryptionService
    {
        /// <summary>
        /// Encrypts a plaintext email address. Non-deterministic: encrypting the same email
        /// twice produces different ciphertext, so this value cannot be used for equality lookups.
        /// </summary>
        string Encrypt(string plainEmail);

        /// <summary>
        /// Decrypts a ciphertext previously produced by <see cref="Encrypt"/>.
        /// </summary>
        string Decrypt(string cipherEmail);

        /// <summary>
        /// Computes a deterministic HMAC-SHA256 "blind index" hash of the normalized email address.
        /// Used as a searchable index (e.g. EF Core equality queries) without exposing plaintext.
        /// </summary>
        string ComputeEmailHash(string email);
    }
}
