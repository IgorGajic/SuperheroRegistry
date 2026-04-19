namespace SuperheroRegistry.Infrastructure.Persistence.Entities
{
    public class PowerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int HeroId { get; set; }
        public HeroEntity HeroEntity { get; set; }
    }
}
