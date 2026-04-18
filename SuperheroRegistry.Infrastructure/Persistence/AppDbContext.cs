using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Domain.Entities;

namespace SuperheroRegistry.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Hero> Heroes { get; set; } = null!; //DbSet<HeroEntity> mora da bude
    public DbSet<Power> Powers { get; set; } = null!; //DbSet<PowerEntity> mora da bude

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setup Identity tables    
        base.OnModelCreating(modelBuilder);
        
        // Hero
        modelBuilder.Entity<Hero>(entity =>
        {
            entity.HasKey(hero => hero.Id);
            entity.Property(hero => hero.Codename).IsRequired().HasMaxLength(100);
            entity.Property(hero => hero.OriginStory).IsRequired();
            entity.Property(hero => hero.Status).HasConversion<string>();
            entity.Property(hero => hero.Race).HasConversion<string>();
            entity.Property(hero => hero.Alignment).HasConversion<string>();

            entity.HasOne<IdentityUser>()
                  .WithMany()
                  .HasForeignKey(hero => hero.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

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