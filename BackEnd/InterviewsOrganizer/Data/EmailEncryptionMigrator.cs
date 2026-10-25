using InterviewsOrganizer.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InterviewsOrganizer.Data
{
    /// <summary>
    /// One-time, idempotent backfill that migrates legacy plaintext AspNetUsers rows to the
    /// encrypted-email + HMAC blind-index scheme. Must run after the migration that adds the
    /// EmailHash/EmailEncryptedAt columns, and before the migration that adds the unique index
    /// on EmailHash (otherwise duplicate/empty hash values could violate the constraint).
    ///
    /// Reads/writes raw SQL directly (bypassing EF's Email/NormalizedEmail value converters) because
    /// legacy rows still hold plaintext at this point; running them through the converter would either
    /// double-encrypt or throw while trying to "decrypt" plaintext.
    /// </summary>
    public static class EmailEncryptionMigrator
    {
        public static async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var emailEncryptionService = serviceProvider.GetRequiredService<IEmailEncryptionService>();
            var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();

            var connection = (SqlConnection)dbContext.Database.GetDbConnection();
            var ownsConnection = connection.State != System.Data.ConnectionState.Open;
            if (ownsConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                var pendingUsers = new List<(string Id, string? Email, string? UserName)>();

                const string selectSql = """
                    SELECT Id, Email, UserName
                    FROM AspNetUsers
                    WHERE EmailEncryptedAt IS NULL
                    """;

                await using (var selectCommand = new SqlCommand(selectSql, connection))
                await using (var reader = await selectCommand.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        pendingUsers.Add((
                            reader.GetString(0),
                            reader.IsDBNull(1) ? null : reader.GetString(1),
                            reader.IsDBNull(2) ? null : reader.GetString(2)));
                    }
                }

                if (pendingUsers.Count == 0)
                {
                    logger.LogInformation("EmailEncryptionMigrator: no pending users to migrate.");
                    return;
                }

                logger.LogInformation("EmailEncryptionMigrator: migrating {Count} user row(s) to encrypted email + blind index.", pendingUsers.Count);

                const string updateSql = """
                    UPDATE AspNetUsers
                    SET Email = @Email,
                        NormalizedEmail = @NormalizedEmail,
                        EmailHash = @EmailHash,
                        EmailEncryptedAt = @EmailEncryptedAt,
                        UserName = @UserName,
                        NormalizedUserName = @NormalizedUserName
                    WHERE Id = @Id
                    """;

                foreach (var user in pendingUsers)
                {
                    var plainEmail = user.Email ?? string.Empty;
                    var encryptedEmail = emailEncryptionService.Encrypt(plainEmail);
                    var normalizedEncryptedEmail = emailEncryptionService.Encrypt(plainEmail.ToUpperInvariant());
                    var emailHash = emailEncryptionService.ComputeEmailHash(plainEmail);

                    // Legacy rows use the email address as UserName, which is plaintext PII living in
                    // Identity's own UserName/NormalizedUserName columns. Replace it with an opaque value.
                    var userName = user.UserName;
                    var needsUserNameScrub = !string.IsNullOrEmpty(userName) && userName.Contains('@');
                    var newUserName = needsUserNameScrub ? $"user_{Guid.NewGuid()}" : userName ?? $"user_{Guid.NewGuid()}";

                    await using var updateCommand = new SqlCommand(updateSql, connection);
                    updateCommand.Parameters.AddWithValue("@Email", (object?)encryptedEmail ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@NormalizedEmail", (object?)normalizedEncryptedEmail ?? DBNull.Value);
                    updateCommand.Parameters.AddWithValue("@EmailHash", emailHash);
                    updateCommand.Parameters.AddWithValue("@EmailEncryptedAt", DateTime.UtcNow);
                    updateCommand.Parameters.AddWithValue("@UserName", newUserName);
                    updateCommand.Parameters.AddWithValue("@NormalizedUserName", newUserName.ToUpperInvariant());
                    updateCommand.Parameters.AddWithValue("@Id", user.Id);

                    await updateCommand.ExecuteNonQueryAsync(cancellationToken);
                }

                logger.LogInformation("EmailEncryptionMigrator: migration complete.");
            }
            finally
            {
                if (ownsConnection)
                {
                    await connection.CloseAsync();
                }
            }
        }
    }
}
