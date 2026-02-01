using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;
using System.Net;
using System.Text.Json;

namespace DoMinhGiaBao__SE1856_A01_BE.Middleware
{
    /// <summary>
    /// Global Exception Handler - Catches all unhandled exceptions
    /// Converts exceptions to consistent ApiResponse format with 500 status
    /// </summary>
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandler(
            RequestDelegate next, 
            ILogger<GlobalExceptionHandler> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            // Set response status and content type
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Create error response
            var response = ApiResponse.ErrorResponse(
                "Internal server error",
                _environment.IsDevelopment() 
                    ? new List<string> { exception.Message, exception.StackTrace ?? "No stack trace" }
                    : new List<string> { "An unexpected error occurred. Please try again later." }
            );

            // Serialize and write response
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    /// <summary>
    /// Extension method to register Global Exception Handler
    /// </summary>
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}
