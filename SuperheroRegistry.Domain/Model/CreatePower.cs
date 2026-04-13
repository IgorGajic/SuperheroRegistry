using System.ComponentModel.DataAnnotations;

namespace SuperheroRegistry.Domain.Model
{
    public class CreatePower
    {
        public int HeroId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
