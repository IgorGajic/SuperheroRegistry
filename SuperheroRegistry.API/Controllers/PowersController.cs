using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Api.Model;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Model;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// All endpoints require authorization.
/// </summary>
[ApiController]
[Authorize]
[Route("api/heroes/")]
public class PowersController : ControllerBase
{
    private readonly IHeroService _heroService;
    private readonly IAuthenticationService _authenticationService;

    public PowersController(IHeroService heroService, IAuthenticationService authenticationService)
    {
        _heroService = heroService;
        _authenticationService = authenticationService;
    }

    [HttpPost("{heroId}/powers")]
    public async Task<ActionResult<Hero>> AddPower(int heroId, CreatePowerModel createPowerModel)
    {
        var hero = await _heroService.GetByIdAsync(heroId);
        var userId = _authenticationService.GetUserIdFromClaims(User);
        
        if (hero.UserId != userId)
            return StatusCode(403, "You can only add powers to your own heroes.");

        var createPower = new CreatePower
        {
            HeroId = heroId,
            Name = createPowerModel.Name,
            Description = createPowerModel.Description
        };

        var updatedHero = await _heroService.AddPowerAsync(createPower);
        return Ok(updatedHero);
    }
   
    [HttpDelete("{heroId}/powers/{powerId}")]
    public async Task<IActionResult> RemovePower(int heroId, int powerId)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        var hero = await _heroService.GetByIdAsync(heroId);

        if (hero.UserId != userId)
            return Forbid("You can only remove powers from your own heroes.");

        await _heroService.RemovePowerAsync(heroId, powerId);
        return NoContent();
    }
}