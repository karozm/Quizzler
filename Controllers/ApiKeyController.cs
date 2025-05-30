using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Data;
using MvcPracownicy.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MvcPracownicy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiKeyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateApiKey()
        {
            if (HttpContext.Session.GetString("_IsLoggedIn") != "true")
            {
                return Unauthorized();
            }

            var userId = int.Parse(HttpContext.Session.GetString("_UserId") ?? "0");
            if (userId == 0)
            {
                return Unauthorized("User not found in session");
            }

            var user = await _context.Logins.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Generate a random API key
            var key = GenerateSecureKey();

            var apiKey = new ApiKey
            {
                Key = key,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddYears(1), // Key expires in 1 year
                IsActive = true
            };

            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            return Ok(new { apiKey = key });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeApiKey(string key)
        {
            if (HttpContext.Session.GetString("_IsLoggedIn") != "true")
            {
                return Unauthorized();
            }

            var userId = int.Parse(HttpContext.Session.GetString("_UserId") ?? "0");
            if (userId == 0)
            {
                return Unauthorized("User not found in session");
            }

            var apiKey = await _context.ApiKeys
                .FirstOrDefaultAsync(k => k.Key == key && k.UserId == userId);

            if (apiKey == null)
            {
                return NotFound("API key not found");
            }

            apiKey.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "API key revoked successfully" });
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListApiKeys()
        {
            if (HttpContext.Session.GetString("_IsLoggedIn") != "true")
            {
                return Unauthorized();
            }

            var userId = int.Parse(HttpContext.Session.GetString("_UserId") ?? "0");
            if (userId == 0)
            {
                return Unauthorized("User not found in session");
            }

            var apiKeys = await _context.ApiKeys
                .Where(k => k.UserId == userId)
                .Select(k => new
                {
                    k.Key,
                    k.CreatedAt,
                    k.ExpiresAt,
                    k.IsActive,
                    DaysRemaining = k.ExpiresAt != null ? (int?)(k.ExpiresAt.Value - DateTime.UtcNow).TotalDays : null
                })
                .ToListAsync();

            return Ok(apiKeys);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateApiKey(string key)
        {
            var apiKey = await _context.ApiKeys
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.Key == key && k.IsActive &&
                    (!k.ExpiresAt.HasValue || k.ExpiresAt > DateTime.UtcNow));

            if (apiKey == null)
            {
                return NotFound("Invalid or expired API key");
            }

            return Ok(new
            {
                isValid = true,
                userId = apiKey.UserId,
                expiresAt = apiKey.ExpiresAt,
                daysRemaining = apiKey.ExpiresAt != null ? (int?)(apiKey.ExpiresAt.Value - DateTime.UtcNow).TotalDays : null
            });
        }

        private string GenerateSecureKey()
        {
            using var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}