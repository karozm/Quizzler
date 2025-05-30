using System.ComponentModel.DataAnnotations;

namespace MvcPracownicy.Models
{
    public class Login
    {
        public int Id { get; set; }
        public string LoginName { get; set; } = string.Empty;
        public string Haslo { get; set; } = string.Empty;
        public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
    }
}

