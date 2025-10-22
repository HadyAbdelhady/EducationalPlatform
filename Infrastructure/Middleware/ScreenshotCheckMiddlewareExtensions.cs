using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Middleware
{
    public static class ScreenshotCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseScreenshotCheck(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ScreenshotCheckMiddleware>();
        }
    }
}