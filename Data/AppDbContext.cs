using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Models;



namespace MvcPracownicy.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Dane> Dany { get; set; }
    }
}