using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Api.Model.RequestModels;
using SuperheroRegistry.Api.Model.ResponseModels;
using SuperheroRegistry.API.Interfaces;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Model;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// All endpoints require authorization.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PowersController : ControllerBase
{
    private readonly IHeroService _heroService;
    private readonly IAuthenticationService _authenticationService;

    public PowersController(IHeroService heroService, IAuthenticationService authenticationService)
    {
        _heroService = heroService;
        _authenticationService = authenticationService;
    }

    [HttpPost("{heroId}")]
    public async Task<ActionResult<PowerResponse>> AddPower(int heroId, CreatePowerModel createPowerModel)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        if(userId == null)
            return Unauthorized("User ID not found in token.");
        
        var hero = await _heroService.GetByIdAsync(heroId);
        if(hero == null)
            return NotFound($"Hero with ID {heroId} not found.");

        if (hero.UserId != userId)
            return Forbid("You can only add powers to your own heroes.");

        var createPower = new CreatePower
        {
            HeroId = heroId,
            Name = createPowerModel.Name,
            Description = createPowerModel.Description
        };

        var updatedHero = await _heroService.AddPowerAsync(hero, createPower);
        var powerResponse = MapPowerToResponse(updatedHero.Powers.Last());
        return Ok(powerResponse);
    }
   
    [HttpDelete("{heroId}/{powerId}")]
    public async Task<IActionResult> RemovePower(int heroId, int powerId)
    {
        var userId = _authenticationService.GetUserIdFromClaims(User);
        if(userId == null)
            return Unauthorized("User ID not found in token.");

        var hero = await _heroService.GetByIdAsync(heroId);

        if (hero == null)
                return NotFound($"Hero with ID {heroId} not found.");

        if (hero.UserId != userId)
            return Forbid("You can only remove powers from your own heroes.");

        await _heroService.RemovePowerAsync(hero, powerId);
        return NoContent();
    }

    private static PowerResponse MapPowerToResponse(Power power)
    {
        return new PowerResponse
        {
            Id = power.Id,
            Name = power.Name,
            Description = power.Description,
            HeroId = power.HeroId
        };
    }
}