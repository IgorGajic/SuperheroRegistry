namespace SuperheroRegistry.Application.DTOs
{
    public class HeroDto
    {
        public int Id { get; set; }
        public string Codename { get; set; }
        public string OriginStory { get; set; }
        public string Race { get; set; }
        public string Alignment { get; set; }
        public string Status { get; set; }
        public List<PowerDto> Powers { get; set; }
        public HeroDto(string codename, string originStory, string race, string alignment, string status, List<PowerDto> powers)
        {
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
            Status = status;
            Powers = powers;
        }
        public HeroDto(int id, string codename, string originStory, string race, string alignment, string status, List<PowerDto> powers)
        {
            Id = id;
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
            Status = status;
            Powers = powers;
        }

        public HeroDto()
        {
        }
    }
}
