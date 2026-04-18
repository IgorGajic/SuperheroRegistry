using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;

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
            _appDbContext.Heroes.Add(hero);
            await _appDbContext.SaveChangesAsync();
            return hero;
        }

        public async Task<bool> CodenameExistsAsync(string codename)
        {
            return await _appDbContext.Heroes
                .AnyAsync(h => h.Codename == codename);
        }
        public async Task DeleteAsync(Hero hero)
        {
            _appDbContext.Heroes.Remove(hero);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<Hero>> GetAllAsync()
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .ToListAsync();
        }
        public async Task<Hero?> GetByIdAsync(int id)
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        public async Task<List<Hero>> GetRegisteredAsync()
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .Where(h => h.Status == HeroStatus.Registered)
                .OrderBy(h => h.Codename)
                .ToListAsync();
        }
        public async Task<List<Hero>> GetByUserIdAsync(string userId)
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Codename)
                .ToListAsync();
        }
        public async Task<Hero> UpdateAsync(Hero hero)
        {
            _appDbContext.Heroes.Update(hero);
            await _appDbContext.SaveChangesAsync();
            return hero;
        }
    }
}
