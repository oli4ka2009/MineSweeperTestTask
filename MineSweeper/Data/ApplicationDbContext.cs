using Microsoft.EntityFrameworkCore;
using MineSweeper.Models;

namespace MineSweeper.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<GameResult> GameResults { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
