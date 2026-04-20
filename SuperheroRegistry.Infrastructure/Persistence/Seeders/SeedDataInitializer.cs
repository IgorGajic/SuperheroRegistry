using Microsoft.AspNetCore.Identity;
using SuperheroRegistry.Domain.Model;
using SuperheroRegistry.Infrastructure.Persistence.Entities;

namespace SuperheroRegistry.Infrastructure.Persistence.Seeders;

public class SeedDataInitializer(AppDbContext dbContext, UserManager<User> userManager)
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly UserManager<User> _userManager = userManager;
    public async Task InitializeAsync()
    {
        if (_dbContext.HeroEntities.Any())
        {
            return;
        }

        var userId = await CreateSeederUserAsync();
        await SeedHeroesAsync(userId);

        await _dbContext.SaveChangesAsync();
    }

    private async Task<string> CreateSeederUserAsync()
    {
        const string username = "Seeder123";
        const string password = "seeder123";

        var user = new User
        {
            UserName = username
        };

        await _userManager.CreateAsync(user, password);

        return user.Id;
    }

    private async Task SeedHeroesAsync(string userId)
    {
        var aldric = new HeroEntity
        {
            UserId = userId,
            Codename = "Aldric Stormbinder",
            OriginStory = "A former battle-mage who patrols the borderlands after the Ember War destroyed his order.",
            Race = "Human",
            Alignment = "LawfulGood",
            Status = "Registered",
            PowerEntities = []
        };

        var briar = new HeroEntity
        {
            UserId = userId,
            Codename = "Briar of Thornvale",
            OriginStory = "Raised among woodland healers, Briar hunts those who poison rivers and silence villages.",
            Race = "Elf",
            Alignment = "NeutralGood",
            Status = "Registered",
            PowerEntities = []
        };

        var kora = new HeroEntity
        {
            UserId = userId,
            Codename = "Kora Ironhand",
            OriginStory = "Ex–arena champion seeking redemption after a corrupt guildmaster stole her name.",
            Race = "Dwarf",
            Alignment = "ChaoticNeutral",
            Status = "Draft",
            PowerEntities = []
        };

        await _dbContext.HeroEntities.AddRangeAsync(aldric, briar, kora);

        aldric.PowerEntities =
        [
            new() { Name = "Arc Lightning", Description = "Chained lightning", HeroId = aldric.Id, HeroEntity = aldric },
            new() { Name = "Storm Ward", Description = "Temporary magic barrier", HeroId = aldric.Id, HeroEntity = aldric }
        ];

        briar.PowerEntities =
        [
            new() { Name = "Vine Grasp", Description = "Restraining vines", HeroId = briar.Id, HeroEntity = briar },
            new() { Name = "Keen Senses", Description = "Tracking & detection", HeroId = briar.Id, HeroEntity = briar }
        ];

        kora.PowerEntities =
        [
            new() { Name = "Iron Fists", Description = "Shatter stone and metal", HeroId = kora.Id, HeroEntity = kora }
        ];
    }
}
