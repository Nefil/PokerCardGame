using Microsoft.EntityFrameworkCore;
using PokerCardGame.Models;

namespace PokerCardGame.Data
{
    public class GameDbContext : DbContext
    {
        public DbSet<Players> Players { get; set; }

        private readonly string _connectionString;

        public GameDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Players>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PlayerName).IsRequired();
                entity.Property(e => e.Money).IsRequired();    
            });
        }
    }
}
