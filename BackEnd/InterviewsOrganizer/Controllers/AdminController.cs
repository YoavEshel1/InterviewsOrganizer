using InterviewsOrganizer.Data;
using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using InterviewsOrganizer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserCleanupService _userCleanupService;
        private readonly ITokenService _tokenService;
        private readonly IAuditLogService _auditLogService;
        private readonly AppDbContext _context;
        private readonly IEmailEncryptionService _emailEncryptionService;

        public AdminController(
            UserManager<AppUser> userManager,
            IUserCleanupService userCleanupService,
            ITokenService tokenService,
            IAuditLogService auditLogService,
            AppDbContext context,
            IEmailEncryptionService emailEncryptionService)
        {
            _userManager = userManager;
            _userCleanupService = userCleanupService;
            _tokenService = tokenService;
            _auditLogService = auditLogService;
            _context = context;
            _emailEncryptionService = emailEncryptionService;
        }

        /// <summary>
        /// Creates a new user and assigns them a role. SuperAdmin only.
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var emailHash = _emailEncryptionService.ComputeEmailHash(dto.Email);
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash);
            if (existingUser is not null) return BadRequest("Email is already registered.");

            var user = new AppUser { UserName = $"user_{Guid.NewGuid()}", Email = dto.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Role);
            return Ok();
        }

        /// <summary>
        /// Gets all users. SuperAdmin only.
        /// </summary>
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new { u.Id, u.Email })
                .ToList();
            return Ok(users);
        }

        /// <summary>
        /// Deletes a user by ID. SuperAdmin only.
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userExists = await _userManager.FindByIdAsync(id);
            if (userExists is null) return NotFound();

            var result = await _userCleanupService.DeleteUserAndDataAsync(id);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }

        /// <summary>
        /// Starts an impersonation session for the target user. SuperAdmin only.
        /// Returns a short-lived access token scoped to the target user; all actions
        /// are recorded in the audit log for GDPR compliance.
        /// </summary>
        [HttpPost("impersonate")]
        public async Task<IActionResult> Impersonate(ImpersonateDto dto, CancellationToken cancellationToken)
        {
            var targetEmailHash = _emailEncryptionService.ComputeEmailHash(dto.TargetEmail);
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == targetEmailHash, cancellationToken);
            if (targetUser is null) return NotFound("Target user not found.");

            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            await _auditLogService.LogAsync(
                adminUserId,
                targetUser.Id,
                "User_Impersonation_Started",
                ipAddress,
                cancellationToken);

            var accessToken = _tokenService.GenerateImpersonationToken(targetUser, adminUserId);

            return Ok(new { accessToken });
        }
    }
}