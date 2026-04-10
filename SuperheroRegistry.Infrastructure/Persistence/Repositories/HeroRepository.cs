using Microsoft.EntityFrameworkCore;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
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
        /// Note: Changes are not saved until the transaction is committed.
        /// </summary>
        /// <param name="hero">The hero entity to save.</param>
        /// <returns>The hero to be saved (not yet persisted to database).</returns>
        public async Task<Hero> AddAsync(Hero hero)
        {
            _appDbContext.Heroes.Add(hero);
            return await Task.FromResult(hero);
        }

        /// <summary>
        /// Checks if a hero with the given codename already exists
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
        /// Note: Changes are not saved until the transaction is committed.
        /// </summary>
        /// <param name="hero">The hero to delete.</param>
        public async Task DeleteAsync(Hero hero)
        {
            _appDbContext.Heroes.Remove(hero);
            await Task.CompletedTask;
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
        /// Retrieves all heroes created by a specific user, including their powers.
        /// </summary>
        /// <param name="userId">The user ID to filter by.</param>
        /// <returns>List of heroes belonging to the user.</returns>
        public async Task<List<Hero>> GetByUserIdAsync(string userId)
        {
            return await _appDbContext.Heroes
                .Include(h => h.Powers)
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.Codename)
                .ToListAsync();
        }

        /// <summary>
        /// Updates an existing hero in the database.
        /// Note: Changes are not saved until the transaction is committed.
        /// </summary>
        /// <param name="hero">The hero with updated values.</param>
        /// <returns>The updated hero (not yet persisted to database).</returns>
        public async Task<Hero> UpdateAsync(Hero hero)
        {
            _appDbContext.Heroes.Update(hero);
            return await Task.FromResult(hero);
        }
    }
}
