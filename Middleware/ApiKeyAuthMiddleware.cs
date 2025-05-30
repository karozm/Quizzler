using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Data;
using MvcPracownicy.Models;
using System.Threading.Tasks;

namespace MvcPracownicy.Middleware
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            string apiKey = context.Request.Headers["X-API-Key"].ToString();

            if (string.IsNullOrEmpty(apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            var key = await dbContext.ApiKeys
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.Key == apiKey && k.IsActive &&
                    (!k.ExpiresAt.HasValue || k.ExpiresAt > DateTime.UtcNow));

            if (key == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            // Add user info to the context for use in controllers
            context.Items["User"] = key.User;
            await _next(context);
        }
    }
}