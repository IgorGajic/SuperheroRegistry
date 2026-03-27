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
        public List<PowerDto> Powers { get; set; } = new();
        public string UserId { get; set; } = string.Empty;

        public HeroDto(int id, string codename, string originStory, string race, string alignment, string status, List<PowerDto> powers, string userId)
        {
            Id = id;
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
            Status = status;
            Powers = powers;
            UserId = userId;
        }

        public HeroDto()
        {
            UserId = string.Empty;
        }

        public HeroDto(string codename, string originStory, string race, string alignment, string status, List<PowerDto> powers, string userId)
        {
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
            Status = status;
            Powers = powers;
            UserId = userId;
        }
    }
}
