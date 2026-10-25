using Microsoft.AspNetCore.Identity;

namespace InterviewsOrganizer.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>
        /// Deterministic HMAC-SHA256 "blind index" of the normalized email address.
        /// Used for all equality lookups since Email/NormalizedEmail are encrypted at rest.
        /// </summary>
        public string EmailHash { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when this row's Email/NormalizedEmail were last encrypted. Null means the row
        /// still holds legacy plaintext and is pending migration by EmailEncryptionMigrator.
        /// </summary>
        public DateTime? EmailEncryptedAt { get; set; }
    }
}