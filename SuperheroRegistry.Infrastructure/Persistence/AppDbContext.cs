using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Infrastructure.Persistence.Entities;

namespace SuperheroRegistry.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<HeroEntity> HeroeEntities { get; set; } = null!; 
    public DbSet<PowerEntity> PowerEntities { get; set; } = null!; 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Setup Identity tables    
        base.OnModelCreating(modelBuilder);
        
        // Hero
        modelBuilder.Entity<HeroEntity>(entity =>
        {
            entity.HasKey(hero => hero.Id);
            entity.Property(hero => hero.Codename).IsRequired();
            entity.Property(hero => hero.OriginStory).IsRequired();
            entity.Property(hero => hero.Status).IsRequired();
            entity.Property(hero => hero.Race).IsRequired();
            entity.Property(hero => hero.Alignment).IsRequired();

            entity.HasOne<IdentityUser>()
                  .WithMany()
                  .HasForeignKey(hero => hero.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(hero => hero.PowerEntities)
                  .WithOne(power => power.HeroEntity)
                  .HasForeignKey(power => power.HeroId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Power
        modelBuilder.Entity<PowerEntity>(entity =>
        {
            entity.HasKey(power => power.Id);
            entity.Property(power => power.Name).IsRequired();
            entity.Property(power => power.Description).IsRequired();
        });
    }
}