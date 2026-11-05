using System.Text.Json.Serialization;

namespace InterviewsOrganizer.Models.DTOs
{
    /// <summary>
    /// Response payload from Cloudflare's /turnstile/v0/siteverify endpoint.
    /// </summary>
    public class TurnstileVerifyResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public List<string>? ErrorCodes { get; set; }
    }
}
