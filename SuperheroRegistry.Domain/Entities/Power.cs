namespace SuperheroRegistry.Domain.Entities
{
    public class Power : BaseEntity
    {

        public string Name { get; set; }
        public string Description { get; set; }

        //Foreign key heroid
        public int HeroId { get; set; }
        public Hero Hero { get; set; } = null!;
    }
}
