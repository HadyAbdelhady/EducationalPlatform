using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Middleware
{
    public class ScreenshotCheckMiddleware(RequestDelegate next, EducationDbContext context)
    {
        private readonly RequestDelegate _next = next;
        private readonly EducationDbContext _context = context;

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip middleware for authentication endpoints
            if (context.Request.Path.StartsWithSegments("/api/auth") ||
                context.Request.Path.StartsWithSegments("/api/studentAuth") ||
                context.Request.Path.StartsWithSegments("/api/instructorAuth"))
            {
                await _next(context);
                return;
            }

            // Get user ID from claims
            var userId = context.User?.FindFirst("userId")?.Value;
            
            if (userId != null)
            {
                // Check if user is a student and has tried screenshot
                var student = await _context.Students
                                     .FirstOrDefaultAsync(s => s.UserId.ToString() == userId);

                if (student != null && student.TriedScreenshot)
                {
                    context.Response.StatusCode = 403; // Forbidden
                    context.Response.ContentType = "application/json";
                    var payload = new
                    {
                        error = "Access denied",
                        message = "Your account has been restricted due to attempted screenshot violation"
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    await context.Response.WriteAsync(json);
                    return;
                }
            }

            await _next(context);
        }
    }
}