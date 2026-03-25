using SuperheroRegistry.Application.Interfaces;

namespace SuperheroRegistry.Infrastructure.Persistence.Repositories
{
    public class HeroRepository : IHeroRepository
    {
        public Task<Hero> AddAsync(Hero hero)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CodenameExistsAsync(string codename)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Hero hero)
        {
            throw new NotImplementedException();
        }

        public Task<List<Hero>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Hero?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Hero>> GetRegisteredAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Hero> UpdateAsync(Hero hero)
        {
            throw new NotImplementedException();
        }
    }
}
