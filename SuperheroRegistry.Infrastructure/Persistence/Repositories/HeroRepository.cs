using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Infrastructure.Persistence.Entities;

namespace SuperheroRegistry.Infrastructure.Persistence.Repositories
{
    public class HeroRepository : IHeroRepository
    {
        private readonly AppDbContext _appDbContext;
        public HeroRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Hero> AddAsync(Hero hero)
        {
            var heroEntity = new HeroEntity
            {
                Codename = hero.Codename,
                OriginStory = hero.OriginStory,
                UserId = hero.UserId,
                Race = hero.Race.ToString(),
                Alignment = hero.Alignment.ToString(),
                Status = hero.Status.ToString(),
                //novi heroji nemaju moci
                PowerEntities =  []
            };

            _appDbContext.HeroEntities.Add(heroEntity);
            await _appDbContext.SaveChangesAsync();
            hero.Id = heroEntity.Id;
            return hero;
        }

        public async Task<bool> CodenameExistsAsync(string codename)
        {
            return await _appDbContext.HeroEntities
                .AnyAsync(h => h.Codename == codename);
        }
        public async Task DeleteAsync(Hero hero)
        {
            var heroEntity = GetHeroEntityByHeroId(hero.Id) 
                ?? throw new KeyNotFoundException($"Hero with id {hero.Id} not found.");

            _appDbContext.HeroEntities.Remove(heroEntity);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<Hero>> GetAllHeroesAsync()
        {
            var heroEntity = await _appDbContext.HeroEntities
                .Include(h => h.PowerEntities)
                .ToListAsync();

            return heroEntity.Select(MapEntityToHero).ToList();
        }
        public async Task<Hero?> GetByIdAsync(int id)
        {
            var entity = await _appDbContext.HeroEntities
                .Include(h => h.PowerEntities)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (entity == null)
                return null;
            return MapEntityToHero(entity);
        }
        public async Task<List<Hero>> GetRegisteredAsync()
        {
            var entities = await _appDbContext.HeroEntities
                .Include(h => h.PowerEntities)
                .Where(h => h.Status == HeroStatus.Registered.ToString())
                .OrderBy(h => h.Codename)
                .ToListAsync();

            return entities.Select(MapEntityToHero).ToList();
        }
        public async Task<List<Hero>> GetByUserIdAsync(string userId)
        {
            var entities = await _appDbContext.HeroEntities
                .Include(h => h.PowerEntities)
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Codename)
                .ToListAsync();

            return entities.Select(MapEntityToHero).ToList();
        }
        public async Task<Hero> UpdateAsync(Hero hero)
        {
            var heroEntity = await GetHeroEntityByHeroIdWithPowersAsync(hero.Id)
                ?? throw new KeyNotFoundException($"Hero with id {hero.Id} not found.");

            heroEntity.Codename = hero.Codename;
            heroEntity.OriginStory = hero.OriginStory;
            heroEntity.Race = hero.Race.ToString();
            heroEntity.Alignment = hero.Alignment.ToString();
            heroEntity.Status = hero.Status.ToString();

            // Remove powers that are no longer in the domain model
            var powersToRemove = heroEntity.PowerEntities
                .Where(powerEntity => !hero.Powers.Any(p => p.Id == powerEntity.Id))
                .ToList();
            foreach (var power in powersToRemove)
            {
                heroEntity.PowerEntities.Remove(power);
            }

            // Add new powers that don't exist in the database (Id == 0)
            var newPowerMap = new Dictionary<Power, PowerEntity>();
            var newPowers = hero.Powers.Where(p => p.Id == 0).ToList();

            foreach (var power in newPowers)
            {
                var powerEntity = new PowerEntity
                {
                    Name = power.Name,
                    Description = power.Description,
                    HeroId = hero.Id
                };
                heroEntity.PowerEntities.Add(powerEntity);
                newPowerMap[power] = powerEntity;
            }

            _appDbContext.HeroEntities.Update(heroEntity);
            await _appDbContext.SaveChangesAsync();

            // Sync generated IDs back to domain objects
            foreach (var (power, powerEntity) in newPowerMap)
            {
                power.Id = powerEntity.Id;
            }

            return hero;
        }


        private HeroEntity? GetHeroEntityByHeroId(int heroId)
        {
            return _appDbContext.HeroEntities.Find(heroId);
        }

        private async Task<HeroEntity?> GetHeroEntityByHeroIdWithPowersAsync(int heroId)
        {
            return await _appDbContext.HeroEntities
                .Include(h => h.PowerEntities)
                .FirstOrDefaultAsync(h => h.Id == heroId);
        }

        private Hero MapEntityToHero(HeroEntity entity)
        {
            List<Power> powers = entity.PowerEntities?.Select(p => new Power(
                p.Name,
                p.Description,
                p.HeroId
            )
            {
                Id = p.Id
            }
            ).ToList() ?? [];

            var hero = new Hero(
                entity.Codename,
                entity.OriginStory,
                Enum.Parse<Race>(entity.Race),
                Enum.Parse<Alignment>(entity.Alignment),
                entity.UserId
            )
            {
                Id = entity.Id,
                Status = Enum.Parse<HeroStatus>(entity.Status),
                Powers = powers
            };
            foreach(var power in hero.Powers)
            {
                power.Hero = hero;
            }
            return hero;
        }
    }
}