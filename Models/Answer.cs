using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Content { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }
    }
}