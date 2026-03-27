using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Application.DTOs;
using SuperheroRegistry.Application.Interfaces;
using System.Security.Claims;

namespace SuperheroRegistry.API.Controllers;

/// <summary>
/// Controller for managing superhero powers.
/// Provides endpoints for adding and removing powers from heroes.
/// All endpoints require authorization.
/// </summary>
[ApiController]
[Authorize]
[Route("api/heroes/{heroId}/powers")]
public class PowersController : ControllerBase
{
    private readonly IHeroService _heroService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PowersController"/> class.
    /// </summary>
    /// <param name="heroService">The hero service for managing hero operations.</param>
    public PowersController(IHeroService heroService)
    {
        _heroService = heroService;
    }

    /// <summary>
    /// Adds a power to a specific hero with authorization.
    /// </summary>
    /// <param name="heroId">The ID of the hero to add the power to.</param>
    /// <param name="dto">The data transfer object containing power creation information.</param>
    /// <returns>The updated hero DTO with the new power added.</returns>
    [HttpPost]
    public async Task<ActionResult<HeroDto>> AddPower(int heroId, CreatePowerDto dto)
    {
        var hero = await _heroService.GetByIdAsync(heroId);

        if (hero.UserId != GetUserId())
            return Forbid("You can only add powers to your own heroes.");

        var updatedHero = await _heroService.AddPowerAsync(heroId, dto);
        return Ok(updatedHero);
    }

    /// <summary>
    /// Removes a power from a specific hero with authorization.
    /// </summary>
    /// <param name="heroId">The ID of the hero to remove the power from.</param>
    /// <param name="powerId">The ID of the power to remove.</param>
    /// <returns>A 204 No Content response on successful removal.</returns>
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