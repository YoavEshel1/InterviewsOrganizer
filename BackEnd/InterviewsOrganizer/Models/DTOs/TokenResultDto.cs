namespace InterviewsOrganizer.Models.DTOs
{
    /// <summary>
    /// Pair of tokens issued on successful login/refresh. The access token is returned to the
    /// caller as the response body; the refresh token is set as an HttpOnly cookie by the controller.
    /// </summary>
    public record TokenResultDto(string AccessToken, string RefreshToken);
}
