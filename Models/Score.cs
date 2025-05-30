using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcPracownicy.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int MaxPoints { get; set; }

        [Required]
        public DateTime CompletedAt { get; set; }

        [ForeignKey("UserId")]
        public Login User { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
    }
}