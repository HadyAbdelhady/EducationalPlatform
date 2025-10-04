using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Edu_Base.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // This protects all endpoints in this controller
    public class ProfileController : ControllerBase
    {
        /// <summary>
        /// Example protected endpoint - requires JWT token
        /// </summary>
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            // Extract user information from the JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            return Ok(new
            {
                userId = userId,
                email = email,
                role = role,
                fullName = fullName,
                message = "This is a protected endpoint - you can only access this with a valid JWT token"
            });
        }

        /// <summary>
        /// Example endpoint that only students can access
        /// </summary>
        [HttpGet("student-only")]
        [Authorize(Roles = "Student")]
        public IActionResult StudentOnly()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                message = $"Hello Student {fullName}!",
                userId = userId,
                info = "Only students can access this endpoint"
            });
        }

        /// <summary>
        /// Example endpoint that only instructors can access
        /// </summary>
        [HttpGet("instructor-only")]
        [Authorize(Roles = "Instructor")]
        public IActionResult InstructorOnly()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                message = $"Hello Instructor {fullName}!",
                userId = userId,
                info = "Only instructors can access this endpoint"
            });
        }

        /// <summary>
        /// Example endpoint accessible by both students and instructors
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Student,Instructor")]
        public IActionResult Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullName = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = $"Welcome to your dashboard, {fullName}!",
                userId = userId,
                role = role,
                info = "Both students and instructors can access this endpoint"
            });
        }
    }
}
