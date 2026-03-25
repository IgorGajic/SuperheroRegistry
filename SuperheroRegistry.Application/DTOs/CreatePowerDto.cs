namespace SuperheroRegistry.Application.DTOs
{
    public class CreatePowerDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CreatePowerDto(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
