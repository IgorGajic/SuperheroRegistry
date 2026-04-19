using System.ComponentModel.DataAnnotations;

namespace SuperheroRegistry.Api.Model.RequestModels
{
    public class CreateHeroModel
    {
        [Required(ErrorMessage = "Codename is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Codename must be between 3 and 100 characters.")]
        public string Codename { get; set; }

        [Required(ErrorMessage = "Origin story is required.")]
        [StringLength(1000, MinimumLength = 30, ErrorMessage = "Origin story must be between 30 and 1000 characters.")]
        public string OriginStory { get; set; }

        [Required(ErrorMessage = "Race is required.")]
        public string Race { get; set; }

        [Required(ErrorMessage = "Alignment is required.")]
        public string Alignment { get; set; }

        public CreateHeroModel(string codename, string originStory, string race, string alignment)
        {
            Codename = codename;
            OriginStory = originStory;
            Race = race;
            Alignment = alignment;
        }

        public CreateHeroModel()
        {
        }
    }
}
