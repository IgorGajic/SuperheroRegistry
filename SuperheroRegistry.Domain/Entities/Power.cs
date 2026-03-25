namespace SuperheroRegistry.Domain.Entities
{
    public class Power : BaseEntity
    {

        public string Name { get; set; }
        public string Description { get; set; }

        //Foreign key: HeroId
        public int HeroId { get; set; }
        public Hero Hero { get; set; } = null!;
        public Power(string name, string description, int heroId, Hero hero)
        {
            if(hero.Id != heroId)
                throw new ArgumentException("Hero ID does not match the provided hero entity.");

            Name = name;
            Description = description;
            HeroId = heroId;
            Hero = hero;
        }

        //Za EF
        protected Power() { }

        public Power(string name, string description, int heroId)
        {
            Name = name;
            Description = description;
            HeroId = heroId;
        }
    }
}
