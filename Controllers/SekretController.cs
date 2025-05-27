using Microsoft.AspNetCore.Mvc;
using MvcPracownicy.Data;
using MvcPracownicy.Models;

namespace MvcPracownicy.Controllers
{
    public class SekretController : Controller
    {
        private readonly AppDbContext _context;

        public SekretController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("_IsLoggedIn") != "true")
            {
                return RedirectToAction("Logowanie", "IO");
            }

            var dane = _context.Dany.ToList();
            return View(dane);
        }

        public IActionResult Add()
        {
            return View(new Dane());
        }

        [HttpPost]
        public IActionResult Add(Dane dane)
        {
            if (ModelState.IsValid)
            {
                _context.Dany.Add(dane);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dane);
        }
    }
}