
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
        Task<Hero> RegisterAsync(int id, string userId);
        Task<Hero> RetireAsync(int id, string userId);
        Task DeleteAsync(int id, string userId);
        Task<Hero> AddPowerAsync(CreatePower createPower);
        Task RemovePowerAsync(int heroId, int powerId);
        Task<bool> CodenameExistsAsync(string codename);
    }
}
