using EAMS_ACore.AuthModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;

namespace EAMS.Middleware
{
    public class TokenExpirationMiddleware
    {
        private readonly RequestDelegate _next;
        public TokenExpirationMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            if (context.User.Identity.IsAuthenticated)
            {

                // Resolve scoped services within the request pipeline
                var userManager = serviceProvider.GetRequiredService<UserManager<UserRegistration>>();
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var userRole = context.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;


                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
                {
                    await _next(context);
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await userManager.FindByIdAsync(userId);
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    if (user?.CurrentToken != token)
                    {
                        context.Response.StatusCode = 440; // Custom status code for session expired
                        var response = new
                        {
                            Code = 440,
                            Message = "Session expired. Please log in again."
                        };

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                        return;
                    }
                }
            }
            await _next(context);
        }
    }


    public static class TokenExpirationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenExpirationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenExpirationMiddleware>();
        }
    }
}
