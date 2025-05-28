using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}