using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Data;
using MvcPracownicy.Models;
using System.Text.Json;

namespace MvcPracownicy.Controllers
{
    public class SolveController : Controller
    {
        private readonly AppDbContext _context;

        public SolveController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

        public async Task<IActionResult> Solve(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, Dictionary<string, string[]> answers)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            int score = 0;

            foreach (var question in quiz.Questions)
            {
                if (answers.ContainsKey($"{question.Id}"))
                {
                    string answerId = answers[$"{question.Id}"][0];
                    var answer = question.Answers.FirstOrDefault(a => a.Id == int.Parse(answerId));
                    if (answer != null && answer.IsCorrect)
                    {
                        score++;
                    }
                }
            }

            double percentageScore = (double)score / quiz.Questions.Count * 100;
            ViewData["Score"] = Math.Round(percentageScore, 2);
            ViewData["QuizTitle"] = quiz.Title;

            string message;
            if (percentageScore <= 30)
            {
                message = "beznadziejny wynik";
            }
            else if (percentageScore <= 60)
            {
                message = "może być";
            }
            else if (percentageScore <= 90)
            {
                message = "jest dobrze";
            }
            else
            {
                message = "foka";
            }
            ViewData["Message"] = message;

            TempData["UserAnswers"] = JsonSerializer.Serialize(answers);
            TempData["QuizId"] = id;

            return View("Result");
        }

        public async Task<IActionResult> ViewAnswers(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var userAnswersJson = TempData["UserAnswers"] as string;
            var userAnswers = userAnswersJson != null 
                ? JsonSerializer.Deserialize<Dictionary<string, string[]>>(userAnswersJson) 
                : new Dictionary<string, string[]>();

            ViewData["UserAnswers"] = userAnswers;

            return View(quiz);
        }
    }
} 