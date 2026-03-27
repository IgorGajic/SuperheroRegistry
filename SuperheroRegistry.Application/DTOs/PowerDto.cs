namespace SuperheroRegistry.Application.DTOs
{
    public class PowerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public PowerDto()
        {
        }

        public PowerDto(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
