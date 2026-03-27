using SuperheroRegistry.Application.DTOs;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;

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
        /// <param name="heroId">The ID of the hero to add the power to.</param>
        /// <param name="dto">The power details (name and description).</param>
        /// <returns>Updated hero DTO with all powers.</returns>
        /// <exception cref="KeyNotFoundException">If the hero does not exist.</exception>
        public async Task<HeroDto> AddPowerAsync(int heroId, CreatePowerDto dto)
        {
            var hero = await _heroRepository.GetByIdAsync(heroId)
                ?? throw new KeyNotFoundException($"Hero with id {heroId} not found.");

            var power = new Power(dto.Name, dto.Description, heroId, hero);
            hero.AddPower(power);
            await _heroRepository.UpdateAsync(hero);
            return MapToDto(hero);
        }

        /// <summary>
        /// Creates a new hero in Draft status.
        /// </summary>
        /// <param name="dto">The hero details (codename, origin story, race, alignment).</param>
        /// <param name="userId">The ID of the user creating the hero.</param>
        /// <returns>The newly created hero DTO.</returns>
        /// <exception cref="ArgumentException">If race or alignment enum values are invalid.</exception>
        public async Task<HeroDto> CreateAsync(CreateHeroDto dto, string userId)
        {
            if (!Enum.TryParse<Race>(dto.Race, ignoreCase:true, out var race))
                throw new ArgumentException($"Invalid race: {dto.Race}");

            if (!Enum.TryParse<Alignment>(dto.Alignment, ignoreCase:true, out var alignment))
                throw new ArgumentException($"Invalid alignment: {dto.Alignment}");

            var hero = new Hero(dto.Codename, dto.OriginStory, race, alignment);
           
            hero.UserId = userId;

            var saved = await _heroRepository.AddAsync(hero);
            return MapToDto(saved);
        }

        /// <summary>
        /// Deletes a draft hero. Only heroes that have not been registered can be deleted.
        /// </summary>
        /// <param name="id">The hero ID.</param>
        /// <param name="userId">The user ID (for authorization check).</param>
        /// <exception cref="KeyNotFoundException">If the hero does not exist.</exception>
        /// <exception cref="InvalidOperationException">If the hero is not in Draft status.</exception>
        /// <exception cref="UnauthorizedAccessException">If the user is not the hero's owner.</exception>
        public async Task DeleteAsync(int id, string userId)
        {
            var hero = await _heroRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Hero with id {id} not found.");

            if (hero.Status != HeroStatus.Draft)
                throw new InvalidOperationException("Only draft heroes can be deleted.");

            if (hero.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own heroes.");

            await _heroRepository.DeleteAsync(hero);
        }

        /// <summary>
        /// Retrieves all heroes (Draft, Registered, and Retired).
        /// </summary>
        /// <returns>List of all hero DTOs.</returns>
        public async Task<List<HeroDto>> GetAllAsync()
        {
            var heroes = await _heroRepository.GetAllAsync();
            return heroes.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Retrieves a specific hero by ID.
        /// </summary>
        /// <param name="id">The hero ID.</param>
        /// <returns>The hero DTO.</returns>
        /// <exception cref="KeyNotFoundException">If the hero does not exist.</exception>
        public async Task<HeroDto> GetByIdAsync(int id)
        {
            var hero = await _heroRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Hero with ID {id} not found.");
            return MapToDto(hero);
        }

        /// <summary>
        /// Retrieves all heroes with Registered status.
        /// </summary>
        /// <returns>List of registered hero DTOs.</returns>
        public async Task<List<HeroDto>> GetRegisteredAsync()
        {
            var heroes = await _heroRepository.GetRegisteredAsync();
            return heroes.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Registers a hero for active duty. Validates all business rules before transitioning to Registered status.
        /// </summary>
        /// <param name="id">The hero ID.</param>
        /// <returns>The registered hero DTO.</returns>
        /// <exception cref="KeyNotFoundException">If the hero does not exist.</exception>
        /// <exception cref="DomainException">If validation fails (thrown by Hero.Register).</exception>
        public async Task<HeroDto> RegisterAsync(int id)
        {
            var hero = await _heroRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Hero with id {id} not found.");

            hero.Register();
            await _heroRepository.UpdateAsync(hero);
            return MapToDto(hero);
        }

        /// <summary>
        /// Removes a power from a hero by power ID.
        /// </summary>
        /// <param name="heroId">The hero ID.</param>
        /// <param name="powerId">The power ID to remove.</param>
        /// <exception cref="KeyNotFoundException">If the hero does not exist.</exception>
        /// <exception cref="DomainException">If the removal violates business rules.</exception>
        public async Task RemovePowerAsync(int heroId, int powerId)
        {
            var hero = await _heroRepository.GetByIdAsync(heroId)
                       ?? throw new KeyNotFoundException($"Hero with id {heroId} not found.");

            var removed = hero.RemovePower(powerId);
            if (!removed)
            {
                throw new KeyNotFoundException($"Power with id {powerId} not found for hero with id {heroId}.");
            }
            await _heroRepository.UpdateAsync(hero);
        }

        /// <summary>
        /// Retires an active hero from duty. Transitions status from Registered to Retired.
        /// </summary>
        /// <param name="id">The hero ID.</param>
        /// <returns>The retired hero DTO.</returns>
        /// <exception cref="KeyNotFoundException">If the hero does not exist.</exception>
        /// <exception cref="DomainException">If the hero is not in Registered status.</exception>
        public async Task<HeroDto> RetireAsync(int id)
        {
            var hero = await _heroRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Hero with id {id} not found.");

            hero.Retire();
            await _heroRepository.UpdateAsync(hero);
            return MapToDto(hero);
        }
        private static HeroDto MapToDto(Hero hero)
        {
            return new()
            {
                Id = hero.Id,
                Codename = hero.Codename,
                OriginStory = hero.OriginStory,
                Race = hero.Race.ToString(),
                Alignment = hero.Alignment.ToString(),
                Status = hero.Status.ToString(),
                Powers = hero.Powers.Select(p => new PowerDto(p.Id, p.Name, p.Description)).ToList()
            };
        }
    }
}
