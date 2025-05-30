using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class ApiKey
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; } = string.Empty;

        public int UserId { get; set; }
        public Login? User { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
