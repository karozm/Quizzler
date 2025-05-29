using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Data;
using MvcPracownicy.Models;
using System.Security.Cryptography;
using System.Text;

namespace MvcPracownicy.Controllers
{
    public class DataManagerController : Controller
    {
        private readonly AppDbContext _context;

        public DataManagerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Sprawdź czy użytkownik jest zalogowany jako admin
            if (HttpContext.Session.GetString("_UserRole") != "admin")
            {
                return RedirectToAction("Logowanie", "IO");
            }

            return View();
        }

        public async Task<IActionResult> Users()
        {
            // Sprawdź czy użytkownik jest zalogowany jako admin
            if (HttpContext.Session.GetString("_UserRole") != "admin")
            {
                return RedirectToAction("Logowanie", "IO");
            }

            var users = await _context.Logins.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(string loginName, string password)
        {
            if (HttpContext.Session.GetString("_UserRole") != "admin")
            {
                return RedirectToAction("Logowanie", "IO");
            }

            if (!string.IsNullOrEmpty(loginName) && !string.IsNullOrEmpty(password))
            {
                var newUser = new Login
                {
                    LoginName = loginName,
                    Haslo = GetMd5Hash(password)
                };

                await _context.Logins.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("_UserRole") != "admin")
            {
                return RedirectToAction("Logowanie", "IO");
            }

            var user = await _context.Logins.FindAsync(id);
            if (user != null)
            {
                _context.Logins.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Users));
        }

        private static string GetMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
} 