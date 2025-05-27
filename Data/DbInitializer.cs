using MvcPracownicy.Models;
using System.Security.Cryptography;
using System.Text;

namespace MvcPracownicy.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Logins.Any())
            {
                context.Logins.AddRange(
                    new Login { LoginName = "admin", Haslo = GetMd5Hash("admin123") },
                    new Login { LoginName = "user", Haslo = GetMd5Hash("user123") }
                );
            }

            if (!context.Dany.Any())
            {
                context.Dany.AddRange(
                    new Dane { Tekst = "Przykładowy wpis 1" },
                    new Dane { Tekst = "Drugi testowy wpis" }
                );
            }

            context.SaveChanges();
        }

        private static string GetMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}