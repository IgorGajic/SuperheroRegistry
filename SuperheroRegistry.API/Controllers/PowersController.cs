using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Api.Model;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Model;
using System.Security.Claims;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// All endpoints require authorization.
/// </summary>
[ApiController]
[Authorize]
[Route("api/heroes/{heroId}/powers")]
public class PowersController : ControllerBase
{
    private readonly IHeroService _heroService;

    public PowersController(IHeroService heroService)
    {
        _heroService = heroService;
    }

    [HttpPost]
    public async Task<ActionResult<Hero>> AddPower(CreatePowerModel createPowerModel)
    {
        var hero = await _heroService.GetByIdAsync(createPowerModel.HeroId);

        if (hero.UserId != GetUserId())
            return StatusCode(403, "You can only add powers to your own heroes.");

        var createPower = new CreatePower
        {
            HeroId = createPowerModel.HeroId,
            Name = createPowerModel.Name,
            Description = createPowerModel.Description
        };

        var updatedHero = await _heroService.AddPowerAsync(createPower);
        return Ok(updatedHero);
    }
   
    [HttpDelete("{powerId}")]
    public async Task<IActionResult> RemovePower(int heroId, int powerId)
    {
        var hero = await _heroService.GetByIdAsync(heroId);

        if (hero.UserId != GetUserId())
            return Forbid("You can only remove powers from your own heroes.");

        await _heroService.RemovePowerAsync(heroId, powerId);
        return NoContent();
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found in token.");
}