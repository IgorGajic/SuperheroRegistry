using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Exceptions;
using SuperheroRegistry.Domain.Model;

namespace SuperheroRegistry.Application.Services
{
    /// <summary>
    /// Orchestrates hero management operations including creation, registration, power management, and retirement.
    /// Handles validation, persistence, and DTO conversion.
    /// </summary>
    public class HeroService : IHeroService
    {
        private readonly IHeroRepository _heroRepository;

        public HeroService(IHeroRepository heroRepository)
        {
            _heroRepository = heroRepository;
        }

        /// <summary>
        /// Adds a new power to an existing hero.
        /// </summary>
        public async Task<Hero> AddPowerAsync(Hero hero, CreatePower createPower)
        {
            if (hero.Status == HeroStatus.Retired)
                throw new DomainException("Cannot manage powers for retired heroes.");

            var power = new Power(createPower.Name, createPower.Description, createPower.HeroId);

            hero.Powers.Add(power);
            return await _heroRepository.UpdateAsync(hero);          
        }

        public async Task<Hero> CreateAsync(CreateHero createHero)
        {
            var hero = new Hero(createHero.Codename, createHero.OriginStory, createHero.Race, createHero.Alignment, createHero.UserId);
            return await _heroRepository.AddAsync(hero);
        }
        public async Task DeleteAsync(Hero hero)
        {
            if (hero.Status != HeroStatus.Draft)
                throw new InvalidOperationException("Only draft heroes can be deleted.");

            await _heroRepository.DeleteAsync(hero);
        }

        public async Task<List<Hero>> GetAllHeroesAsync()
        {
            var heroes = await _heroRepository.GetAllHeroesAsync();
            return heroes;
        }

        public async Task<Hero> GetByIdAsync(int id)
        {
            var hero = await _heroRepository.GetByIdAsync(id);
            return hero;
        }

        public async Task<List<Hero>> GetRegisteredHeroesAsync()
        {
            return await _heroRepository.GetRegisteredAsync();
        }

        public async Task<List<Hero>> GetByUserIdAsync(string userId)
        {
            var heroes = await _heroRepository.GetByUserIdAsync(userId);
            return heroes;
        }

        public async Task<bool> CodenameExistsAsync(string codename)
        {
            return await _heroRepository.CodenameExistsAsync(codename);
        }

        public async Task<Hero> RegisterAsync(Hero hero)
        {
            if (hero.Status != HeroStatus.Draft)
                throw new InvalidOperationException("Only draft heroes can be registered.");

            if (string.IsNullOrWhiteSpace(hero.OriginStory) || hero.OriginStory.Length < HeroConstants.MinimumOriginStoryLength)
                throw new DomainException($"Origin story must be at least '{HeroConstants.MinimumOriginStoryLength}' characters.");

            if(hero.Powers.Count < HeroConstants.MinimumPowerCountForRegisteringHeroes)
                throw new DomainException($"Hero must have at least {HeroConstants.MinimumPowerCountForRegisteringHeroes} power to be registered.");

            foreach (var phrase in HeroConstants.ForbiddenPhrases)
                if (hero.OriginStory.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                    throw new DomainException($"Origin story contains forbidden phrase: '{phrase}'.");

            hero.Status = HeroStatus.Registered;
            return await _heroRepository.UpdateAsync(hero);
        }

        public async Task<Hero> RemovePowerAsync(Hero hero, int powerId)
        {
            if (hero.Status == HeroStatus.Retired)
            {
                throw new DomainException("Cannot manage powers for retired heroes.");
            }

            if (hero.Powers.Count == 1 && hero.Status == HeroStatus.Registered)
            {
                throw new DomainException("Registered hero must have at least 1 power.");
            }

            var power = hero.Powers.FirstOrDefault(p => p.Id == powerId) 
                ?? throw new KeyNotFoundException($"Power with id {powerId} not found for hero with id {hero.Id}.");

            hero.Powers.Remove(power);
            return await _heroRepository.UpdateAsync(hero);
        }

        public async Task<Hero> UpdateAsync(UpdateHero updateHero)
        {
            var hero = await _heroRepository.GetByIdAsync(updateHero.Id)
                ?? throw new KeyNotFoundException($"Hero with id {updateHero.Id} not found.");

            hero.Codename = updateHero.Codename;
            hero.OriginStory = updateHero.OriginStory;
            hero.Race = updateHero.Race;
            hero.Alignment = updateHero.Alignment;

            return await _heroRepository.UpdateAsync(hero);
        }
        public async Task<Hero> RetireAsync(Hero hero)
        {
            if (hero.Status != HeroStatus.Registered)
                throw new DomainException("Only registered heroes can be retired.");

            hero.Status = HeroStatus.Retired;

            await _heroRepository.UpdateAsync(hero);
            return hero;
        }
    }
}
