using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Exceptions;

namespace SuperheroRegistry.Domain.Entities
{
    public class Hero
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Codename { get; set; }
        public string OriginStory { get; set; }
        public Race Race { get; set; }
        public Alignment Alignment { get; set; }
        public HeroStatus Status { get; set; }
        public List<Power> Powers { get; set; }


        public Hero(string codename, string originStory, Race race, Alignment alignment, string userId)
        {
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
            UserId = userId;
            Powers = new List<Power>();
            Status = HeroStatus.Draft;
        }      
    }
}