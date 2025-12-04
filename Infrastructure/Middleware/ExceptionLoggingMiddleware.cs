//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using System.Net;
//using System.Text.Json;

//namespace Infrastructure.Middleware
//{
//    public class ExceptionLoggingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

//        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                // Log full exception (including inner exceptions) for diagnostics
//                _logger.LogError(ex, "Unhandled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);

//                context.Response.Clear();
//                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                context.Response.ContentType = "application/json";

//                var payload = new
//                {
//                    error = "An internal server error occurred.",
//                    message = "A server error occurred. For more details check server logs."
//                };

//                var json = JsonSerializer.Serialize(payload);
//                await context.Response.WriteAsync(json);
//            }
//        }
//    }
//}
