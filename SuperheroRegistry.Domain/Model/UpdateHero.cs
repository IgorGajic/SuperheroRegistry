using SuperheroRegistry.Domain.Enums;

namespace SuperheroRegistry.Domain.Model
{
    public class UpdateHero
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Codename { get; set; }
        public string OriginStory { get; set; }
        public Race Race { get; set; }
        public Alignment Alignment { get; set; }
    }
}
