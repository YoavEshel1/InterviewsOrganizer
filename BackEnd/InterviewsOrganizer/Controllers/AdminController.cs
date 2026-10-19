using InterviewsOrganizer.Models.DTOs;
using InterviewsOrganizer.Models.Entities;
using InterviewsOrganizer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InterviewsOrganizer.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserCleanupService _userCleanupService;

        public AdminController(UserManager<AppUser> userManager, IUserCleanupService userCleanupService)
        {
            _userManager = userManager;
            _userCleanupService = userCleanupService;
        }

        /// <summary>
        /// Creates a new user and assigns them a role. SuperAdmin only.
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var user = new AppUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
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
    }
}