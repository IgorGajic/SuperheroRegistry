using SuperheroRegistry.Application.DTOs;
using SuperheroRegistry.Application.Interfaces;

namespace SuperheroRegistry.Application.Services
{
    public class HeroService : IHeroService
    {
        public Task<HeroDto> AddPowerAsync(int heroId, CreatePowerDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<HeroDto> CreateAsync(CreateHeroDto dto, string userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<HeroDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HeroDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<HeroDto>> GetRegisteredAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HeroDto> RegisterAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task RemovePowerAsync(int heroId, int powerId)
        {
            throw new NotImplementedException();
        }

        public Task<HeroDto> RetireAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
