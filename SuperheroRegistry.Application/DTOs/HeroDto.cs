namespace SuperheroRegistry.Application.DTOs
{
    public class HeroDto
    {
        public int Id { get; set; }
        public string Codename { get; set; } = string.Empty;
        public string OriginStory { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public string Alignment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<PowerDto> Powers { get; set; } = new();

        public HeroDto()
        {
        }

        public HeroDto(int id, string codename, string originStory, string race, string alignment, string status, string userId, List<PowerDto> powers)
        {
            Id = id;
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
            Status = status;
            UserId = userId;
            Powers = powers;
        }
    }
}
