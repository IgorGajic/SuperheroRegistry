namespace SuperheroRegistry.Domain.Entities
{
    public class Power : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Power(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
