using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
        {
            var response = await _authService.RegisterAsync(dto, ct);

            if (!response.Success || response.Tokens is null)
            {
                return BadRequest(response.ErrorMessage);
            }

            SetRefreshTokenCookie(response.Tokens.RefreshToken);

            return Ok(new { profile = response.Profile, accessToken = response.Tokens.AccessToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(dto, ct);
            if (!result.Success || result.Tokens is null)
                return Unauthorized(result.ErrorMessage);

            SetRefreshTokenCookie(result.Tokens.RefreshToken);

            return Ok(new { accessToken = result.Tokens.AccessToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RefreshAsync(refreshToken, ct);
            if (!result.Success || result.Tokens is null)
                return Unauthorized(result.ErrorMessage);

            SetRefreshTokenCookie(result.Tokens.RefreshToken);

            return Ok(new { accessToken = result.Tokens.AccessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            await _authService.LogoutAsync(refreshToken, ct);
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
    }
}
