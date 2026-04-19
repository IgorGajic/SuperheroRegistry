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
            _appDbContext.HeroeEntities.Add(mapHeroToEntity(hero));
            await _appDbContext.SaveChangesAsync();
            return hero;
        }

        public async Task<bool> CodenameExistsAsync(string codename)
        {
            return await _appDbContext.HeroeEntities
                .AnyAsync(h => h.Codename == codename);
        }
        public async Task DeleteAsync(Hero hero)
        {
            _appDbContext.HeroeEntities.Remove(mapHeroToEntity(hero));
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<Hero>> GetAllAsync()
        {
            var heroEntity = await _appDbContext.HeroeEntities
                .Include(h => h.PowerEntities)
                .ToListAsync();

            return heroEntity.Select(mapEntityToHero).ToList();
        }
        public async Task<Hero?> GetByIdAsync(int id)
        {
            var entity = await _appDbContext.HeroeEntities
                .Include(h => h.PowerEntities)
                .FirstOrDefaultAsync(h => h.Id == id);

            return mapEntityToHero(entity);
        }
        public async Task<List<Hero>> GetRegisteredAsync()
        {
            var entities = await _appDbContext.HeroeEntities
                .Include(h => h.PowerEntities)
                .Where(h => h.Status == HeroStatus.Registered.ToString())
                .OrderBy(h => h.Codename)
                .ToListAsync();

            return entities.Select(mapEntityToHero).ToList();
        }
        public async Task<List<Hero>> GetByUserIdAsync(string userId)
        {
            var entities = await _appDbContext.HeroeEntities
                .Include(h => h.PowerEntities)
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Codename)
                .ToListAsync();

            return entities.Select(mapEntityToHero).ToList();
        }
        public async Task<Hero> UpdateAsync(Hero hero)
        {
            _appDbContext.HeroeEntities.Update(mapHeroToEntity(hero));
            await _appDbContext.SaveChangesAsync();
            return hero;
        }


        private HeroEntity mapHeroToEntity(Hero hero)
        {
            if(hero == null)
                return null;

            List<PowerEntity> powers = hero.Powers?.Select(p => new PowerEntity
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                HeroId = hero.Id
            }).ToList() ?? [];

            return new HeroEntity
            {
                Id = hero.Id,
                Codename = hero.Codename,
                OriginStory = hero.OriginStory,
                Status = hero.Status.ToString(),
                Race = hero.Race.ToString(),
                Alignment = hero.Alignment.ToString(),
                UserId = hero.UserId,
                PowerEntities = powers
            };
        }

        private Hero mapEntityToHero(HeroEntity entity)
        {
            List<Power> powers = entity.PowerEntities?.Select(p => new Power(
                p.Name,
                p.Description,
                p.HeroId
            )).ToList() ?? [];

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
            hero.Status = Enum.Parse<HeroStatus>(entity.Status);
            foreach(var power in hero.Powers)
            {
                power.Hero = hero;
            }
            return hero;
        }
    }
}