using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;

namespace SuperheroRegistry.Infrastructure.Persistence.Entities
{
    public class HeroEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Codename { get; set; }
        public string OriginStory { get; set; }
        public string Race { get; set; }
        public string Alignment { get; set; }
        public string Status { get; set; }
        public List<PowerEntity> PowerEntities { get; set; }
    }
}
