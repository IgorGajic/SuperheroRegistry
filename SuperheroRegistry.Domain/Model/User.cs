using Microsoft.AspNetCore.Identity;

namespace SuperheroRegistry.Domain.Model;

public class User : IdentityUser
{
    // Uzima ove fieldove iz ugradjene IdentityUser klase, kapiram da je bolje ovo nego sam da je pravim (osim sto imamo visak fieldova)
    // - Id
    // - UserName
    // - PasswordHash
}
