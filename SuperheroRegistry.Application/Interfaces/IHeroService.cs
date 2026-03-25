
using SuperheroRegistry.Application.DTOs;

namespace SuperheroRegistry.Application.Interfaces
{
    public interface IHeroService
    {
        Task<List<HeroDto>> GetAllAsync();
        Task<List<HeroDto>> GetRegisteredAsync();
        Task<HeroDto> GetByIdAsync(int id);
        Task<HeroDto> CreateAsync(CreateHeroDto dto, int userId);
        Task<HeroDto> RegisterAsync(int id);
        Task<HeroDto> RetireAsync(int id);
        Task DeleteAsync(int id, int userId);
        Task<HeroDto> AddPowerAsync(int heroId, CreatePowerDto dto);
        Task RemovePowerAsync(int heroId, int powerId);
    }
}
