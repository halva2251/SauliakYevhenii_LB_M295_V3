using Microsoft.EntityFrameworkCore;
using SauliakYevheniiLB_295API.Models;

namespace SauliakYevheniiLB_295API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Hero> Heroes { get; set; }
    public DbSet<Ability> Abilities { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Hero Configuration
        modelBuilder.Entity<Hero>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Role).IsRequired().HasMaxLength(20);

            // One-to-Many: Hero -> Abilities
            entity.HasMany(h => h.Abilities)
                  .WithOne(a => a.Hero)
                  .HasForeignKey(a => a.HeroId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Ability Configuration
        modelBuilder.Entity<Ability>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
        });

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(u => u.Username).IsUnique();
        });

    }
}