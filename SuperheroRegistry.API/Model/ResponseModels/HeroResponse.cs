using System.ComponentModel.DataAnnotations;
using SuperheroRegistry.Domain.Entities;

namespace SuperheroRegistry.Api.Model.ResponseModels
{
    public class HeroResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Codename { get; set; }
        public string OriginStory { get; set; }
        public string Status { get; set; }
        public string Race { get; set; }
        public string Alignment { get; set; }
        public List<Power> Powers { get; set; }

    }
}
