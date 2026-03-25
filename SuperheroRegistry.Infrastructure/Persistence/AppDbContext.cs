using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Domain.Entities;

namespace SuperheroRegistry.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Hero> Heroes { get; set; } = null!;
    public DbSet<Power> Powers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hero
        modelBuilder.Entity<Hero>(entity =>
        {
            entity.HasKey(hero => hero.Id);
            entity.Property(hero => hero.Codename).IsRequired().HasMaxLength(100);
            entity.Property(hero => hero.OriginStory).IsRequired();
            entity.Property(hero => hero.Status).HasConversion<string>();
            entity.Property(hero => hero.Race).HasConversion<string>();
            entity.Property(hero => hero.Alignment).HasConversion<string>();

            entity.HasMany(hero => hero.Powers)
                  .WithOne(power => power.Hero)
                  .HasForeignKey(power => power.HeroId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Power
        modelBuilder.Entity<Power>(entity =>
        {
            entity.HasKey(power => power.Id);
            entity.Property(power => power.Name).IsRequired();
            entity.Property(power => power.Description).IsRequired();
        });
    }
}