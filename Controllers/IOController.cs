using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MvcPracownicy.Data;
using MvcPracownicy.Models;

namespace MvcPracownicy.Controllers
{
    public class IOController : Controller
    {
        private const string SessionKeyLoggedIn = "_IsLoggedIn";
        private const string SessionKeyRole = "_UserRole";
        private const string SessionKeyUserId = "_UserId";
        private readonly AppDbContext _context;

        public IOController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Logowanie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logowanie(string login, string haslo)
        {
            // Sprawdź czy to admin
            if (login == "admin" && haslo == "admin123")
            {
                HttpContext.Session.SetString(SessionKeyLoggedIn, "true");
                HttpContext.Session.SetString(SessionKeyRole, "admin");
                HttpContext.Session.SetString(SessionKeyUserId, "1"); // Admin ma ID 1
                return RedirectToAction("Zalogowano");
            }

            // Sprawdź zwykłego użytkownika
            var user = _context.Logins.SingleOrDefault(l => l.LoginName == login);
            if (user != null && user.Haslo == GetMd5Hash(haslo))
            {
                HttpContext.Session.SetString(SessionKeyLoggedIn, "true");
                HttpContext.Session.SetString(SessionKeyRole, "user");
                HttpContext.Session.SetString(SessionKeyUserId, user.Id.ToString());
                return RedirectToAction("Zalogowano");
            }

            ViewBag.Blad = "Nieprawidłowy login lub hasło.";
            return View();
        }

        public IActionResult Zalogowano()
        {
            if (HttpContext.Session.GetString(SessionKeyLoggedIn) != "true")
            {
                return RedirectToAction("Logowanie");
            }

            ViewBag.IsAdmin = HttpContext.Session.GetString(SessionKeyRole) == "admin";
            return View();
        }

        [HttpPost]
        public IActionResult Wyloguj()
        {
            HttpContext.Session.Remove(SessionKeyLoggedIn);
            HttpContext.Session.Remove(SessionKeyRole);
            HttpContext.Session.Remove(SessionKeyUserId);
            return RedirectToAction("Logowanie");
        }

        private static string GetMd5Hash(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}