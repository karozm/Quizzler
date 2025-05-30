using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Data;
using MvcPracownicy.Models;

namespace MvcPracownicy.Controllers
{
    public class ScoresController : Controller
    {
        private readonly AppDbContext _context;

        public ScoresController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var scores = await _context.Scores
                .Include(s => s.User)
                .Include(s => s.Quiz)
                .OrderByDescending(s => s.CompletedAt)
                .ToListAsync();

            return View(scores);
        }

        public async Task<IActionResult> UserScores(int userId)
        {
            var scores = await _context.Scores
                .Include(s => s.Quiz)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CompletedAt)
                .ToListAsync();

            return View(scores);
        }
    }
}