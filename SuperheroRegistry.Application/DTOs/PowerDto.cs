namespace SuperheroRegistry.Application.DTOs
{
    public class PowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PowerDto(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public PowerDto(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
