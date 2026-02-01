using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Net;
using System.Text.Json;

namespace DoMinhGiaBao__SE1856_A01_BE.Middleware
{
    /// <summary>
    /// Custom Authorization Middleware Result Handler
    /// Converts authorization failures to consistent ApiResponse format
    /// - 401 Unauthorized: No token or invalid token
    /// - 403 Forbidden: Valid token but insufficient permissions
    /// </summary>
    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly IAuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            // If authorization succeeded, continue normally
            if (authorizeResult.Succeeded)
            {
                await next(context);
                return;
            }

            // If user is authenticated but not authorized (403 Forbidden)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";

                var response = ApiResponse.ErrorResponse(
                    "Forbidden",
                    "You do not have permission to access this resource"
                );

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
                return;
            }

            // If user is not authenticated (401 Unauthorized)
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var unauthorizedResponse = ApiResponse.ErrorResponse(
                "Unauthorized",
                "Authentication is required to access this resource"
            );

            var jsonUnauthorizedResponse = JsonSerializer.Serialize(unauthorizedResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonUnauthorizedResponse);
        }
    }
}
