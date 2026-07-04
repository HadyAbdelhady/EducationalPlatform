using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Middleware
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