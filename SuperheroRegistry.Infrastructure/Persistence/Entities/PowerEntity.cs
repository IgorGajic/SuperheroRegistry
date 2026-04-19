namespace SuperheroRegistry.Infrastructure.Persistence.Entities
{
    public class PowerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int HeroId { get; set; }
        public HeroEntity HeroEntity { get; set; }
    }
}
