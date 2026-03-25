namespace SuperheroRegistry.Application.DTOs
{
    public class CreateHeroDto
    {

        public string Codename { get; set; }
        public string OriginStory { get; set; }
        public string Race { get; set; }
        public string Alignment { get; set; }
        public CreateHeroDto(string codename, string originStory, string race, string alignment)
        {
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
        }
    }
}
