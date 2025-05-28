using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}