using System.Security.Claims;
using Application.Common.Interfaces;

namespace Edu_Base.Services
{
    public sealed class CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment environment,
        IConfiguration configuration) : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IHostEnvironment _environment = environment;
        private readonly IConfiguration _configuration = configuration;

        public bool TryGetUserId(out Guid userId)
        {
            if (_environment.IsDevelopment())
            {
                var overrideId = _configuration["Auth:DevUserId"];
                if (!string.IsNullOrWhiteSpace(overrideId) && Guid.TryParse(overrideId, out userId))
                {
                    return true;
                }
            }

            var claim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out userId))
            {
                userId = default;
                return false;
            }

            return true;
        }

        public Guid GetUserId()
        {
            if (TryGetUserId(out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("User id claim is missing.");
        }
    }
}
