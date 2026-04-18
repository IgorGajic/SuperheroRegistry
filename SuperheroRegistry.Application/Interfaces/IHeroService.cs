
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Model;

namespace SuperheroRegistry.Application.Interfaces
{
    public interface IHeroService
    {
        Task<List<Hero>> GetAllAsync();
        Task<List<Hero>> GetRegisteredAsync();
        Task<Hero> GetByIdAsync(int id);
        Task<List<Hero>> GetByUserIdAsync(string userId);
        Task<Hero> CreateAsync(CreateHero createHero);
        Task<Hero> UpdateAsync(UpdateHero updateHero);
        Task<Hero> RegisterAsync(Hero hero);
        Task<Hero> RetireAsync(Hero hero);
        Task DeleteAsync(Hero hero);
        Task<Hero> AddPowerAsync(Hero hero, CreatePower createPower);
        Task RemovePowerAsync(Hero hero, int powerId);
        Task<bool> CodenameExistsAsync(string codename);
    }
}
