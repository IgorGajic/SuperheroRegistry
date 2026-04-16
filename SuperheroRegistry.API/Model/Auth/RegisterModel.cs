using System.ComponentModel.DataAnnotations;

namespace SuperheroRegistry.Api.Model.Auth
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
