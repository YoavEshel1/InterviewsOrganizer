using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IEmailEncryptionService _emailEncryptionService;

        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService, AppDbContext context, IEmailEncryptionService emailEncryptionService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _emailEncryptionService = emailEncryptionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash);
            if (existingUser is not null) return BadRequest("Email is already registered.");

            var user = new AppUser { UserName = $"user_{Guid.NewGuid()}", Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash);
            if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            SetRefreshTokenCookie(refreshToken);

            return Ok(new { accessToken });
        }
    
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

            var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);
            if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            SetRefreshTokenCookie(newRefreshToken);

            return Ok(new { accessToken = newAccessToken });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);
                if (user is not null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiry = null;
                    await _userManager.UpdateAsync(user);
                }
            }
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
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