namespace SuperheroRegistry.Application.Interfaces
{
    public interface IHeroRepository
    {
        Task<Hero?> GetByIdAsync(int id);                  
        Task<List<Hero>> GetAllAsync();
        Task<List<Hero>> GetRegisteredAsync();     
        Task<Hero> AddAsync(Hero hero);
        Task<Hero> UpdateAsync(Hero hero);
        Task DeleteAsync(Hero hero);
        Task<bool> CodenameExistsAsync(string codename);
    }
}
