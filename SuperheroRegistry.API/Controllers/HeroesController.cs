using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperheroRegistry.Api.Model;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Model;
using System.Security.Claims;

namespace SuperheroRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroesController : ControllerBase
{
    private readonly IHeroService _heroService;

    public HeroesController(IHeroService heroService)
    {
        _heroService = heroService;
    }

    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Hero>>> GetRegistered()
    {
        var heroes = await _heroService.GetRegisteredAsync();
        return Ok(heroes);
    }

    [HttpGet("exists/{codename}")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> CodenameExists(string codename)
    {
        var exists = await _heroService.CodenameExistsAsync(codename);
        return Ok(exists);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<Hero>>> GetAll()
    {
        var heroes = await _heroService.GetAllAsync();
        return Ok(heroes);
    }

    /// <summary>
    /// Retrieves all heroes created by the logged-in user
    /// </summary>
    [HttpGet("my-heroes")]
    [Authorize]
    public async Task<ActionResult<List<Hero>>> GetMyHeroes()
    {
        var userId = GetUserId();
        var heroes = await _heroService.GetByUserIdAsync(userId);
        return Ok(heroes);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Hero>> GetById(int id)
    {
        var hero = await _heroService.GetByIdAsync(id);

        if (hero.UserId != GetUserId())
            return StatusCode(403, "You don't have permission to view this hero.");

        return Ok(hero);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateHeroModel>> Create(CreateHeroModel createHeroModel)
    {
        if (!Enum.TryParse<Race>(createHeroModel.Race, ignoreCase: true, out var race))
            throw new ArgumentException($"Invalid race: {createHeroModel.Race}");

        if (!Enum.TryParse<Alignment>(createHeroModel.Alignment, ignoreCase: true, out var alignment))
            throw new ArgumentException($"Invalid alignment: {createHeroModel.Alignment}");

        var createHero = new CreateHero
        {
            UserId = createHeroModel.UserId,
            Codename = createHeroModel.Codename,
            OriginStory = createHeroModel.OriginStory,
            Race = race,
            Alignment = alignment,
        };

        var hero = await _heroService.CreateAsync(createHero);
        return CreatedAtAction(nameof(GetById), new { id = hero.Id }, hero);
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<ActionResult<UpdateHeroModel>> Update(UpdateHeroModel updateHeroModel)
    {
        if (!Enum.TryParse<Race>(updateHeroModel.Race, ignoreCase: true, out var race))
            throw new ArgumentException($"Invalid race: {updateHeroModel.Race}");

        if (!Enum.TryParse<Alignment>(updateHeroModel.Alignment, ignoreCase: true, out var alignment))
            throw new ArgumentException($"Invalid alignment: {updateHeroModel.Alignment}");
        var updateHero = new UpdateHero
        {
            Id = updateHeroModel.Id,
            UserId = updateHeroModel.UserId,
            Codename = updateHeroModel.Codename,
            OriginStory = updateHeroModel.OriginStory,
            Race = race,
            Alignment = alignment
        };
        var hero = await _heroService.UpdateAsync(updateHero);
        return Ok(hero);
    }

    [HttpPatch("{id}/register")]
    [Authorize]
    public async Task<ActionResult<Hero>> Register(int id)
    {
        var hero = await _heroService.RegisterAsync(id, GetUserId());
        return Ok(hero);
    }

    [HttpPatch("{id}/retire")]
    [Authorize]
    public async Task<ActionResult<Hero>> Retire(int id)
    {
        var hero = await _heroService.RetireAsync(id, GetUserId());
        return Ok(hero);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        await _heroService.DeleteAsync(id, GetUserId());
        return NoContent();
    }

    /// <summary>
    /// Retrieves the current authenticated user's ID from the JWT token claims.
    /// </summary>
    /// <returns>The user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID cannot be found in the token.</exception>
    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found in token.");
}