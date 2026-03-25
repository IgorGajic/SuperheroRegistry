using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Infrastructure.Persistence;

namespace SuperheroRegistry.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Handles hero data persistence and retrieval from the database.
    /// Manages CRUD operations and filtering of hero entities.
    /// </summary>
    public class HeroRepository : IHeroRepository
    {
        private readonly AppDbContext _appDbContext;

        public HeroRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Creates and persists a new hero in the database.
        /// </summary>
        /// <param name="hero">The hero entity to save.</param>
        /// <returns>The saved hero with database-generated ID.</returns>
        public async Task<Hero> AddAsync(Hero hero)
        {
            _appDbContext.Heroes.Add(hero);
            await _appDbContext.SaveChangesAsync();
            return hero;
        }

        /// <summary>
        /// Checks if a hero with the given codename already exists (case-insensitive).
        /// </summary>
        /// <param name="codename">The codename to check.</param>
        /// <returns>True if codename exists, false otherwise.</returns>
        public async Task<bool> CodenameExistsAsync(string codename)
        {
            return await _appDbContext.Heroes
                .AnyAsync(h => h.Codename == codename);    
        }

        /// <summary>
        /// Deletes a hero from the database. Associated powers are cascade deleted.
        /// </summary>
        /// <param name="hero">The hero to delete.</param>
        public async Task DeleteAsync(Hero hero)
        {
            _appDbContext.Heroes.Remove(hero);
            await _appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all heroes from the database, including their powers.
        /// </summary>
        /// <returns>List of all heroes with loaded powers.</returns>
        public async Task<List<Hero>> GetAllAsync()
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific hero by ID, including all associated powers.
        /// </summary>
        /// <param name="id">The hero ID.</param>
        /// <returns>The hero with loaded powers, or null if not found.</returns>
        public async Task<Hero?> GetByIdAsync(int id)
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        /// <summary>
        /// Retrieves all heroes with Registered status, sorted by codename.
        /// </summary>
        /// <returns>Sorted list of registered heroes.</returns>
        public async Task<List<Hero>> GetRegisteredAsync()
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .Where(h => h.Status == HeroStatus.Registered)
                .OrderBy(h => h.Codename)
                .ToListAsync();
        }

        /// <summary>
        /// Updates an existing hero in the database.
        /// </summary>
        /// <param name="hero">The hero with updated values.</param>
        /// <returns>The updated hero.</returns>
        public async Task<Hero> UpdateAsync(Hero hero)
        {
            _appDbContext.Heroes.Update(hero);
            await _appDbContext.SaveChangesAsync();
            return hero;
        }
    }
}
