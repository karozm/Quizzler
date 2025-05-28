using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Data;
using MvcPracownicy.Models;

namespace MvcPracownicy.Controllers
{
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Question/Create
        public IActionResult Create(int quizId)
        {
            ViewBag.QuizId = quizId;
            return View();
        }

        // POST: Question/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,QuizId")] Question question, int CorrectAnswerIndex, List<string> Answers)
        {
            try
            {
                if (question == null)
                {
                    ModelState.AddModelError("", "Question data is missing");
                    ViewBag.QuizId = Request.Form["QuizId"];
                    return View();
                }

                if (string.IsNullOrWhiteSpace(question.Content))
                {
                    ModelState.AddModelError("Content", "Question content is required");
                    ViewBag.QuizId = question.QuizId;
                    return View(question);
                }

                if (Answers == null || Answers.Count != 4)
                {
                    ModelState.AddModelError("", "Exactly 4 answers are required");
                    ViewBag.QuizId = question.QuizId;
                    return View(question);
                }

                if (CorrectAnswerIndex < 0 || CorrectAnswerIndex >= 4)
                {
                    ModelState.AddModelError("", "Invalid correct answer selection");
                    ViewBag.QuizId = question.QuizId;
                    return View(question);
                }

                // Add the question
                _context.Add(question);
                await _context.SaveChangesAsync();

                // Add the answers
                for (int i = 0; i < Answers.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(Answers[i]))
                    {
                        ModelState.AddModelError("", $"Answer {i + 1} cannot be empty");
                        ViewBag.QuizId = question.QuizId;
                        return View(question);
                    }

                    var answer = new Answer
                    {
                        Content = Answers[i],
                        IsCorrect = i == CorrectAnswerIndex,
                        QuestionId = question.Id
                    };
                    _context.Add(answer);
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Quiz", new { id = question.QuizId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                ViewBag.QuizId = question?.QuizId ?? int.Parse(Request.Form["QuizId"].ToString());
                return View(question);
            }
        }

        // GET: Question/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Question/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,QuizId")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Quiz", new { id = question.QuizId });
            }
            return View(question);
        }

        // GET: Question/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Quiz", new { id = question?.QuizId });
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}