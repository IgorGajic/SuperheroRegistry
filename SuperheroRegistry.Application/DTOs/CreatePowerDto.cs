using System.ComponentModel.DataAnnotations;

namespace SuperheroRegistry.Application.DTOs
{
    public class CreatePowerDto
    {
        [Required(ErrorMessage = "Power name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Power name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Power description is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Power description must be between 10 and 500 characters.")]
        public string Description { get; set; }

        public CreatePowerDto(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public CreatePowerDto()
        {
        }
    }
}
